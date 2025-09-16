# Contributing to SWITCH

Thank you for your interest in contributing to SWITCH! This document provides guidelines for contributing to the project.

## Development Workflow

### Git Workflow
We follow a feature branch workflow with the following conventions:

1. **Main Branch**: `main` - Production-ready code
2. **Development Branch**: `develop` - Integration branch for features
3. **Feature Branches**: `feature/sprint-x-component-name` - Individual features
4. **Hotfix Branches**: `hotfix/issue-description` - Critical bug fixes

### Branch Naming Convention
- `feature/sprint-1-gamemanager` - New features
- `bugfix/match-detection-fix` - Bug fixes
- `hotfix/critical-crash-fix` - Critical fixes
- `docs/uml-diagrams-update` - Documentation updates
- `refactor/performance-optimization` - Code refactoring

### Commit Message Format
We use conventional commits with the following format:

```
[Sprint X] Type: Description

Detailed description of changes (optional)

Closes #issue-number (if applicable)
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `perf`: Performance improvements
- `chore`: Maintenance tasks

**Examples:**
```
[Sprint 1] feat: Implement GameManager singleton pattern

[Sprint 1] fix: Resolve tile placement edge case

[Sprint 1] docs: Update architecture documentation

[Sprint 1] test: Add GameManager unit tests
```

## Development Setup

### Prerequisites
- Unity 2022.3 LTS
- Git
- Mobile device for testing
- Cursor IDE (recommended)

### Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/uriahwhitt/SWITCH.git
   cd SWITCH
   ```

2. Run the planning context update:
   ```bash
   ./update-planning-context.sh
   ```

3. Open Unity and load the project from `src/` directory

4. Check current sprint status in `planning-context/SPRINT_STATUS.md`

### Development Process
1. **Check Current Status**: Review `planning-context/SPRINT_STATUS.md`
2. **Create Feature Branch**: `git checkout -b feature/sprint-x-component-name`
3. **Implement Feature**: Follow SWITCH_Cursor_rules.md
4. **Run Tests**: `./run-tests.sh`
5. **Update Documentation**: Update relevant docs and sprint status
6. **Commit Changes**: Use conventional commit format
7. **Push Branch**: `git push origin feature/sprint-x-component-name`
8. **Create Pull Request**: Follow PR template

## Code Standards

### C# Standards
- Follow C# naming conventions
- Use PascalCase for classes and methods
- Use camelCase for fields and variables
- Use UPPER_CASE for constants
- Use properties with backing fields
- Use `[SerializeField]` for private fields needing Inspector access

### Documentation Standards
- **XML Documentation**: All public methods, properties, and classes must have XML documentation
- **Inline Comments**: Explain complex algorithms, business logic, and performance optimizations
- **Educational Comments**: Include learning points and architectural decisions
- **Code Examples**: Provide usage examples in documentation
- **Performance Notes**: Document performance implications and optimizations
- **Unity-Specific Notes**: Explain Unity patterns and best practices

### Unity Standards
- Implement object pooling for performance
- Use event-driven architecture
- Follow singleton pattern for managers
- Cache component references in Awake/Start
- Never use Find() methods in Update loops

### Performance Requirements
- Maintain 60 FPS on target devices
- Memory allocation <1KB per frame
- Draw calls <100 total
- Load time <5 seconds

### Testing Requirements
- Write unit tests for all public methods
- Write integration tests for system interactions
- Write performance tests for critical paths
- Test on mobile devices regularly

## Pull Request Process

### PR Template
When creating a pull request, include:

1. **Description**: What changes were made and why
2. **Sprint Reference**: Which sprint and task this addresses
3. **Testing**: How the changes were tested
4. **Performance Impact**: Any performance implications
5. **Documentation**: What documentation was updated
6. **Checklist**: Complete the PR checklist

### PR Checklist
- [ ] Code follows project standards
- [ ] Tests pass (`./run-tests.sh`)
- [ ] Performance targets met
- [ ] **Comprehensive documentation with educational comments**
- [ ] **XML documentation for all public APIs**
- [ ] **Inline comments explaining complex logic**
- [ ] Documentation updated
- [ ] Sprint status updated
- [ ] Planning context updated (`./update-planning-context.sh`)

### Review Process
1. **Automated Checks**: CI/CD pipeline runs tests
2. **Code Review**: Team member reviews code
3. **Performance Review**: Performance impact assessed
4. **Documentation Review**: Documentation completeness checked
5. **Approval**: Approved by team member
6. **Merge**: Merged into target branch

## Issue Reporting

### Bug Reports
When reporting bugs, include:
- **Sprint**: Which sprint the bug was found in
- **Component**: Which component is affected
- **Steps to Reproduce**: Clear reproduction steps
- **Expected Behavior**: What should happen
- **Actual Behavior**: What actually happens
- **Environment**: Device, OS, Unity version
- **Performance Impact**: Any performance implications

### Feature Requests
When requesting features, include:
- **Sprint**: Which sprint this should be in
- **Component**: Which component this affects
- **Description**: Clear feature description
- **Rationale**: Why this feature is needed
- **Acceptance Criteria**: How to know it's complete
- **Performance Impact**: Any performance implications

## Performance Guidelines

### Mobile Optimization
- Test on minimum spec devices (iPhone 6s, Samsung Galaxy S8)
- Profile performance after each feature
- Use object pooling for repeated objects
- Optimize draw calls and memory usage
- Test battery impact

### Profiling Requirements
- Run Unity Profiler for 5 minutes of gameplay
- Check FPS (must stay at 60)
- Check memory allocation per frame (<1KB)
- Check draw calls (<100)
- Document results in sprint status

## Documentation Standards

### Code Documentation
- **XML Documentation**: All public APIs must have comprehensive XML documentation
- **Inline Comments**: Explain complex algorithms, business logic, and performance optimizations
- **Educational Comments**: Include learning points and architectural decisions for future developers
- **Code Examples**: Provide usage examples and patterns in documentation
- **Performance Notes**: Document performance implications and optimization strategies
- **Unity-Specific Notes**: Explain Unity patterns, lifecycle methods, and best practices
- **README Files**: Each major system must have a README with examples and usage
- **Architecture Decision Records (ADRs)**: Document all significant technical decisions

### UML Diagrams
- Update diagrams when architecture changes
- Use Mermaid format for version control
- Export to PNG for documentation
- Include legends for complex diagrams

## Release Process

### Version Numbering
We use semantic versioning: `MAJOR.MINOR.PATCH`
- **MAJOR**: Breaking changes
- **MINOR**: New features
- **PATCH**: Bug fixes

### Release Checklist
- [ ] All tests pass
- [ ] Performance targets met
- [ ] Documentation complete
- [ ] Release notes written
- [ ] Version number updated
- [ ] Build artifacts created
- [ ] Store assets ready

## Getting Help

### Resources
- **Documentation**: Check `docs/` directory
- **Planning Context**: Check `planning-context/` directory
- **Sprint Status**: Check `planning-context/SPRINT_STATUS.md`
- **Architecture**: Check `docs/architecture/` directory

### Communication
- **Issues**: Use GitHub Issues for bugs and features
- **Discussions**: Use GitHub Discussions for questions
- **Pull Requests**: Use PR comments for code discussions

## Code of Conduct

### Our Standards
- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and grow
- Maintain professional communication
- Respect different perspectives

### Enforcement
- Report violations to project maintainers
- Violations will be addressed promptly
- Repeat violations may result in removal

## License

By contributing to SWITCH, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing to SWITCH! 🎮
