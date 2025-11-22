# Development Guidelines

## Code Quality Rules

### Before Every Commit

**MANDATORY STEPS - NO EXCEPTIONS:**

1. **Build the project**
   ```bash
   dotnet build
   ```
   - Must complete without errors
   - Address all compiler warnings

2. **Run tests**
   ```bash
   dotnet test
   ```
   - All tests must pass
   - No skipped critical tests

3. **Verify runtime functionality**
   - Start the application
   - Test affected endpoints/features
   - Verify no runtime errors

### Code Changes Checklist

- [ ] Add necessary `using` statements
- [ ] Build succeeds without errors
- [ ] All tests pass
- [ ] Runtime verification complete
- [ ] Code follows project conventions
- [ ] Documentation updated if needed

### Why This Matters

- Prevents broken commits from being pushed
- Catches missing dependencies immediately
- Ensures CI/CD pipeline success
- Maintains team productivity
- Reduces debugging time for others

## Commit Message Format

```
<type>: <description>

[optional body]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code restructuring
- `test`: Adding tests
- `docs`: Documentation changes
- `chore`: Maintenance tasks

## Testing Standards

- Unit tests for all business logic
- Integration tests for API endpoints
- Minimum 80% code coverage target
- Test edge cases and error conditions

## Pull Request Requirements

1. All commits follow guidelines above
2. Tests included for new features
3. Documentation updated
4. No merge conflicts
5. CI/CD pipeline passes
6. Code review approved
