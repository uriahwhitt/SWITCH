# SWITCH Branch Management Strategy

## Overview
This document defines the Git branch management strategy for the SWITCH project, ensuring organized development workflow and code quality.

## Branch Types

### 1. **main** (Production Branch)
- **Purpose**: Production-ready code, always stable and deployable
- **Protection**: Protected branch, requires PR approval
- **Merge Source**: Only from `develop` or `hotfix/*` branches
- **Release Point**: All releases are tagged from this branch

### 2. **develop** (Integration Branch)
- **Purpose**: Integration branch for features, sprint completion point
- **Protection**: Protected branch, requires PR approval
- **Merge Source**: Feature branches and hotfix branches
- **Sprint Integration**: All sprint features merge here before release

### 3. **feature/sprint-x-component** (Feature Branches)
- **Purpose**: Individual feature development
- **Naming**: `feature/sprint-1-gamemanager`, `feature/sprint-2-queue-system`
- **Source**: Created from `develop`
- **Destination**: Merged into `develop` via PR
- **Lifetime**: Deleted after successful merge

### 4. **hotfix/issue-description** (Hotfix Branches)
- **Purpose**: Critical bug fixes for production
- **Naming**: `hotfix/critical-crash-fix`, `hotfix/performance-regression`
- **Source**: Created from `main`
- **Destination**: Merged into both `main` and `develop`
- **Lifetime**: Deleted after successful merge

### 5. **docs/update-type** (Documentation Branches)
- **Purpose**: Documentation updates and improvements
- **Naming**: `docs/uml-diagrams-update`, `docs/api-documentation`
- **Source**: Created from `develop`
- **Destination**: Merged into `develop` via PR
- **Lifetime**: Deleted after successful merge

### 6. **refactor/component-name** (Refactoring Branches)
- **Purpose**: Code refactoring and optimization
- **Naming**: `refactor/performance-optimization`, `refactor/architecture-cleanup`
- **Source**: Created from `develop`
- **Destination**: Merged into `develop` via PR
- **Lifetime**: Deleted after successful merge

## Workflow Processes

### Feature Development Workflow
```bash
# 1. Start new feature
git checkout develop
git pull origin develop
git checkout -b feature/sprint-1-gamemanager

# 2. Develop feature with regular commits
git add .
git commit -m "[Sprint 1] feat: Implement GameManager singleton pattern"
git push origin feature/sprint-1-gamemanager

# 3. Create Pull Request
# - Target: develop
# - Include comprehensive description
# - Link to sprint task
# - Include test results

# 4. After PR approval and merge
git checkout develop
git pull origin develop
git branch -d feature/sprint-1-gamemanager
```

### Hotfix Workflow
```bash
# 1. Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-crash-fix

# 2. Implement fix with commits
git add .
git commit -m "[Hotfix] fix: Resolve critical crash in tile placement"
git push origin hotfix/critical-crash-fix

# 3. Create PR to main
# - Target: main
# - Include issue description
# - Include fix details

# 4. After merge to main, merge to develop
git checkout develop
git pull origin develop
git merge hotfix/critical-crash-fix
git push origin develop

# 5. Clean up
git branch -d hotfix/critical-crash-fix
```

### Release Workflow
```bash
# 1. Prepare release from develop
git checkout develop
git pull origin develop

# 2. Create release branch (optional)
git checkout -b release/v1.0.0

# 3. Merge to main
git checkout main
git pull origin main
git merge develop
git tag v1.0.0
git push origin main --tags

# 4. Clean up
git checkout develop
git branch -d release/v1.0.0
```

## Branch Naming Conventions

### Feature Branches
- `feature/sprint-1-gamemanager`
- `feature/sprint-2-queue-system`
- `feature/sprint-3-power-ups`
- `feature/sprint-4-backend-integration`

### Hotfix Branches
- `hotfix/critical-crash-fix`
- `hotfix/performance-regression`
- `hotfix/memory-leak-fix`
- `hotfix/input-lag-issue`

### Documentation Branches
- `docs/uml-diagrams-update`
- `docs/api-documentation`
- `docs/architecture-update`
- `docs/performance-guide`

### Refactoring Branches
- `refactor/performance-optimization`
- `refactor/architecture-cleanup`
- `refactor/code-standards`
- `refactor/memory-management`

## Branch Protection Rules

### main Branch
- Require pull request reviews (2 reviewers)
- Require status checks to pass
- Require branches to be up to date
- Restrict pushes to main
- Require linear history

### develop Branch
- Require pull request reviews (1 reviewer)
- Require status checks to pass
- Require branches to be up to date
- Restrict pushes to develop

## Commit Message Standards

### Conventional Commits Format
```
[Sprint X] Type: Description

Detailed description (optional)

Closes #issue-number (if applicable)
```

### Types
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `perf`: Performance improvements
- `chore`: Maintenance tasks

### Examples
```
[Sprint 1] feat: Implement GameManager singleton pattern

[Sprint 1] fix: Resolve tile placement edge case

[Sprint 1] docs: Update architecture documentation

[Sprint 1] test: Add GameManager unit tests

[Hotfix] fix: Resolve critical crash in tile placement

[Docs] docs: Update UML diagrams for Sprint 2
```

## Pull Request Standards

### PR Title Format
```
[Sprint X] Type: Brief Description
```

### PR Description Template
```markdown
## Description
Brief description of changes made and why they were necessary.

## Sprint Reference
- **Sprint**: [Sprint X]
- **Task**: [Task description from sprint plan]
- **Component**: [Component being modified]

## Changes Made
- [ ] List of specific changes
- [ ] Include file paths and line numbers for major changes
- [ ] Note any breaking changes

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Performance tests added/updated
- [ ] Manual testing completed
- [ ] Mobile device testing completed

## Performance Impact
- [ ] FPS impact: [Describe any FPS changes]
- [ ] Memory impact: [Describe any memory changes]
- [ ] Load time impact: [Describe any load time changes]

## Documentation
- [ ] Code documentation updated
- [ ] Architecture documentation updated
- [ ] Sprint status updated
- [ ] Planning context updated

## Checklist
- [ ] Code follows project standards
- [ ] Tests pass
- [ ] Performance targets met
- [ ] Documentation updated
- [ ] Sprint status updated
- [ ] Planning context updated
```

## Branch Lifecycle

### Feature Branch Lifecycle
1. **Creation**: From develop branch
2. **Development**: Regular commits with conventional format
3. **Testing**: Run tests and performance checks
4. **PR Creation**: Create PR to develop
5. **Review**: Code review and approval
6. **Merge**: Merge into develop
7. **Cleanup**: Delete feature branch

### Hotfix Branch Lifecycle
1. **Creation**: From main branch
2. **Development**: Implement fix with commits
3. **Testing**: Verify fix works correctly
4. **PR Creation**: Create PR to main
5. **Review**: Code review and approval
6. **Merge**: Merge into main
7. **Backport**: Merge into develop
8. **Cleanup**: Delete hotfix branch

## Best Practices

### Branch Management
- Keep branches short-lived (max 1-2 weeks)
- Regularly sync with base branch
- Use descriptive branch names
- Delete merged branches promptly
- Use feature flags for incomplete features

### Commit Practices
- Make small, focused commits
- Use conventional commit format
- Include comprehensive commit messages
- Test before committing
- Use interactive rebase for clean history

### PR Practices
- Create PR early for feedback
- Include comprehensive description
- Link to related issues
- Request appropriate reviewers
- Respond to feedback promptly
- Keep PRs focused and small

## Integration with Sprint Workflow

### Sprint Start
- Create feature branches for sprint tasks
- Ensure all branches are based on latest develop
- Update sprint status with branch information

### Sprint Development
- Regular commits to feature branches
- Create PRs when features are complete
- Review and merge PRs into develop
- Update sprint status with progress

### Sprint End
- Merge all completed features into develop
- Create release branch if needed
- Merge develop into main for release
- Tag release version
- Update sprint status with completion

## Conclusion
This branch management strategy ensures organized development workflow, code quality, and efficient collaboration. All team members should follow these guidelines to maintain a clean and efficient Git history.
