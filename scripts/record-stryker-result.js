// Copyright © Erickson Lopez. MIT License.
const fs = require('fs');
const path = require('path');

function loadThresholds(configPath = 'stryker-config.json') {
  let thresholds = { high: 100, low: 98, break: 95 };
  try {
    if (fs.existsSync(configPath)) {
      const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));
      const t = config['stryker-config']?.thresholds || config.thresholds || {};
      thresholds = { high: t.high ?? 100, low: t.low ?? 98, break: t.break ?? 95 };
    }
  } catch (err) {
    console.warn(`Could not parse ${configPath}: ${err.message}`);
  }
  return thresholds;
}

function findJsonReports(dir) {
  let results = [];
  if (!fs.existsSync(dir)) return results;
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  for (const entry of entries) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      results = results.concat(findJsonReports(full));
    } else if (entry.name.endsWith('.json') && !entry.name.endsWith('.html.json') && !entry.name.endsWith('metadata.json') && !entry.name.startsWith('summary-')) {
      results.push(full);
    }
  }
  return results;
}

function hasMutableFiles(configFile) {
  try {
    if (!fs.existsSync(configFile)) return true;
    const config = JSON.parse(fs.readFileSync(configFile, 'utf8'));
    const project = config['stryker-config']?.project || config.project;
    if (!project) return true;

    const srcDir = path.resolve('src');
    let projectDir = null;
    if (fs.existsSync(srcDir)) {
      for (const entry of fs.readdirSync(srcDir, { withFileTypes: true })) {
        if (entry.isDirectory()) {
          const candidate = path.join(srcDir, entry.name, project);
          if (fs.existsSync(candidate)) {
            projectDir = path.join(srcDir, entry.name);
            break;
          }
        }
      }
    }
    if (!projectDir) return true;

    function countCs(dir) {
      let count = 0;
      for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
        if (e.name === 'bin' || e.name === 'obj') continue;
        const full = path.join(dir, e.name);
        if (e.isDirectory()) count += countCs(full);
        else if (e.name.endsWith('.cs') && !e.name.endsWith('.g.cs') && !e.name.endsWith('.AssemblyInfo.cs')) count++;
      }
      return count;
    }

    return countCs(projectDir) > 0;
  } catch {
    return true;
  }
}

function main() {
  const targetDir = process.argv[2] || 'StrykerOutput/ci';
  const pkgName = process.argv[3] || 'DomainPrimitives';
  const configFile = process.argv[4] || 'stryker-config.json';

  const thresholds = loadThresholds(configFile);
  let score = 0;
  let killed = 0;
  let total = 0;
  let foundReport = false;
  let isMetapackage = false;

  const jsonFiles = findJsonReports(targetDir);
  if (jsonFiles.length > 0) {
    try {
      const data = JSON.parse(fs.readFileSync(jsonFiles[0], 'utf8'));
      if (data.mutationScore !== undefined) {
        score = Number(data.mutationScore);
      }
      const files = data.files || {};
      for (const f of Object.values(files)) {
        for (const m of (f.mutants || [])) {
          const st = String(m.status || '').toLowerCase();
          if (st === 'killed' || st === 'timeout') {
            killed++;
            total++;
          } else if (st === 'survived' || st === 'nocoverage') {
            total++;
          }
        }
      }
      if (total > 0 && data.mutationScore === undefined) {
        score = Math.round((killed / total) * 10000) / 100;
      }
      if (total === 0 && !hasMutableFiles(configFile)) {
        score = 100;
        killed = 0;
        total = 0;
        isMetapackage = true;
      }
      foundReport = true;
    } catch (err) {
      console.warn(`Error parsing ${jsonFiles[0]}: ${err.message}`);
    }
  } else if (!hasMutableFiles(configFile)) {
    // Project has zero mutable source files (metapackage)
    score = 100;
    killed = 0;
    total = 0;
    foundReport = true;
    isMetapackage = true;
  }

  const passedGate = (score >= thresholds.break && foundReport) || isMetapackage;
  let statusLabel = '❌ FAILED';
  if (isMetapackage) statusLabel = '✅ HIGH';
  else if (score >= thresholds.high) statusLabel = '✅ HIGH';
  else if (score >= thresholds.low) statusLabel = '🟡 LOW';
  else if (score >= thresholds.break) statusLabel = '🟠 WARNING';

  const sha = process.env.GITHUB_SHA || 'unknown';
  const repo = process.env.GITHUB_REPOSITORY || '';
  const runId = process.env.GITHUB_RUN_ID || '';
  const serverUrl = process.env.GITHUB_SERVER_URL || 'https://github.com';
  const runUrl = repo && runId ? `${serverUrl}/${repo}/actions/runs/${runId}` : '';

  // Save metadata artifact
  const metadata = {
    package: pkgName,
    commit_sha: sha,
    execution_date: new Date().toISOString(),
    mutation_score: score,
    mutants_killed: killed,
    total_mutants: total,
    threshold_high: thresholds.high,
    threshold_low: thresholds.low,
    threshold_break: thresholds.break,
    status: statusLabel,
    passed_break: passedGate,
    run_url: runUrl
  };

  fs.mkdirSync('StrykerOutput', { recursive: true });
  fs.writeFileSync(path.join('StrykerOutput', `summary-${pkgName}.json`), JSON.stringify(metadata, null, 2));

  // Write Step Summary
  const stepSummaryPath = process.env.GITHUB_STEP_SUMMARY;
  if (stepSummaryPath) {
    const summary = `
## 🛡️ Stryker Mutation Testing Results — ${pkgName}

| Metric | Value |
|--------|-------|
| **Mutation Score** | **${score}%** |
| Mutants Killed | ${killed} / ${total} |
| Threshold Break | ≥${thresholds.break}% |
| **Status** | ${statusLabel} |
| Commit SHA | \`${sha.substring(0, 7)}\` |
`;
    fs.appendFileSync(stepSummaryPath, summary);
  }

  // Set GitHub Output
  const outputPath = process.env.GITHUB_OUTPUT;
  if (outputPath) {
    fs.appendFileSync(outputPath, `score=${score}\n`);
    fs.appendFileSync(outputPath, `passed_gate=${passedGate}\n`);
    fs.appendFileSync(outputPath, `status=${statusLabel}\n`);
    fs.appendFileSync(outputPath, `killed=${killed}\n`);
    fs.appendFileSync(outputPath, `total=${total}\n`);
  }

  console.log(`[${pkgName}] Stryker Score: ${score}% (${killed}/${total}) - ${statusLabel}`);
}

main();
