## Description
Briefly describe the changes introduced by this PR.

## Packages Affected
Please check the packages that are affected by this change:
- [ ] `EricksonLopez.DomainPrimitives.Abstractions`
- [ ] `EricksonLopez.DomainPrimitives`
- [ ] `EricksonLopez.DomainPrimitives.Generators`
- [ ] `EricksonLopez.DomainPrimitives.Analyzers`
- [ ] `EricksonLopez.DomainPrimitives.AspNetCore`
- [ ] `EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators`
- [ ] `EricksonLopez.DomainPrimitives.EFCore`
- [ ] `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators`
- [ ] `EricksonLopez.DomainPrimitives.Dapper`
- [ ] `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators`
- [ ] `EricksonLopez.DomainPrimitives.OpenApi`
- [ ] `EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators`
- [ ] `EricksonLopez.DomainPrimitives.NewtonsoftJson`
- [ ] `EricksonLopez.DomainPrimitives.Testing`

## Type of Change
- [ ] 🐛 Bug fix (non-breaking change which fixes an issue)
- [ ] ✨ New feature (non-breaking change which adds functionality)
- [ ] 💥 Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] 📖 Documentation update
- [ ] ⚡ Performance improvement
- [ ] 🎨 Code style / Refactoring

## Performance & Allocation
- [ ] This PR does not introduce any new heap allocations on the hot paths (`TryCreate`, `Parse`, `TryFormat`).
- [ ] If changing the generator core, I have verified benchmark results using `BenchmarkDotNet` and MemoryDiagnoser.

## Quality Gates Checklist
- [ ] My code follows the code style of this project (`WarningLevel 5`, `TreatWarningsAsErrors`).
- [ ] I have updated the documentation accordingly (`README.md`, `/docs/*.md`, XML comments).
- [ ] I have added unit / integration tests to cover my changes.
- [ ] All new and existing tests passed (`dotnet test --collect:"XPlat Code Coverage"`).
- [ ] API Compatibility Check passes (PackageValidation baseline).
- [ ] Codecov and SonarQube analyses pass without dropping thresholds.
- [ ] Stryker mutation testing passes (break threshold: 95%).
- [ ] Native AOT compatibility verified (no IL3050/IL2026 warnings).
- [ ] I have updated `CHANGELOG.md` under `[Unreleased]`.
- [ ] My commit messages follow [Conventional Commits](https://www.conventionalcommits.org/).

## Related Issues
Fixes # (issue)
