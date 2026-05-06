# Copilot Instructions

## Рекомендации по проекту
- Do not create new script files. Ask the user to create them instead, because new classes may not be visible to other scripts due to Unity project structure issues.
- Avoid using `FindFirstObjectByType` and similar `Find*` methods as they are costly and do not guarantee finding the object. Dependencies should be explicitly managed through a bootstrap point or another high-level entity.