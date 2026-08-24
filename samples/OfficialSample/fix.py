import glob
import re

files = glob.glob(r'd:\DevData\ericksonlopez.dev\dotnet-domain-primitives\samples\OfficialSample\**\*.cs', recursive=True)
for f in files:
    with open(f, 'r', encoding='utf-8') as file:
        content = file.read()

    # Restore missing variable in 02-FirstResult
    if '02-FirstResult' in f:
        content = re.sub(r'/\* (Result<Money> transactionResult.*?) \*/', r'\1', content, flags=re.DOTALL)

    # 1. Any 'error.Description' where 'error' is a PrimitiveError -> 'error.Message'
    # 2. Any 'error.Type' where 'error' is PrimitiveError -> 'ErrorType.Validation' 
    content = content.replace('result.Error.Message', 'result.Error.Description')
    content = content.replace('Result.Error.Message', 'Result.Error.Description')
    content = content.replace('Error.Message', 'Error.Description')
    content = content.replace('emailError.Description', 'emailError.Message')
    content = content.replace('error.Description', 'error.Message')
    content = content.replace('error.Type switch', 'ErrorType.Validation switch')

    with open(f, 'w', encoding='utf-8') as file:
        file.write(content)
