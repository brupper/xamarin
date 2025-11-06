# Research: Xamarin.Forms to .NET MAUI Migration

**Feature**: Xamarin.Forms to .NET MAUI Migration  
**Created**: 2025-11-06  
**Status**: Phase 0 Research Complete

## Current State Analysis

### Project Assessment
**Decision**: Brupper.Maui project is already largely migrated to MAUI structure  
**Rationale**: 
- Project file uses `Microsoft.NET.Sdk` with MAUI target frameworks (net9.0-android, net9.0-ios)
- Uses MAUI-specific packages (Microsoft.Maui.Controls, CommunityToolkit.Maui)
- Follows MAUI single-project structure with platform-specific conditionals
- Most active code is already MAUI-compliant

**Alternatives considered**: 
- Complete rewrite from scratch
- Gradual migration approach
- Hybrid approach maintaining both Xamarin.Forms and MAUI support

### Legacy Code in _skip Folder

**Decision**: Migrate valuable components from _skip folder, remove deprecated code  
**Rationale**: The _skip folder contains legacy Xamarin.Forms code that should be evaluated for migration or removal:
- Some pages and platform effects may still be needed
- Custom renderers need conversion to MAUI handlers
- Platform-specific setup code needs MAUI equivalents

**Alternatives considered**:
- Delete all _skip folder contents
- Leave _skip folder as-is for reference
- Gradually migrate all contents

### Package Dependencies Strategy

**Decision**: Use latest stable MAUI packages, replace any Xamarin.Forms remnants  
**Rationale**: 
- Current packages are already MAUI-focused
- Some commented packages (Xamarin.CommunityToolkit) need MAUI equivalents
- Package versions should align with .NET 9 support

**Alternatives considered**:
- Keep some Xamarin.Forms packages for compatibility
- Use preview/beta MAUI packages for latest features
- Minimal package updates only

### Testing Strategy

**Decision**: Create comprehensive test suite for MAUI-specific functionality  
**Rationale**: 
- Current project lacks visible test coverage
- MAUI platform-specific code needs validation
- Breaking changes need automated detection

**Alternatives considered**:
- Manual testing only
- Platform-specific test apps
- Unit tests without UI testing

### Platform Targeting Strategy

**Decision**: Maintain current platform targets (Android, iOS, Windows) with .NET 9  
**Rationale**: 
- Existing project already targets net9.0 frameworks
- These platforms cover primary MAUI scenarios
- Windows support adds value for desktop scenarios

**Alternatives considered**:
- Add macOS and Tizen support
- Focus only on mobile platforms
- Target older .NET versions for compatibility

### Migration Approach for Custom Renderers

**Decision**: Convert custom renderers to MAUI handlers where applicable  
**Rationale**: 
- MAUI handlers are the recommended approach for platform customization
- Renderers in _skip folder need evaluation for current relevance
- Some effects may have built-in MAUI alternatives

**Alternatives considered**:
- Keep renderer approach using compatibility packages
- Remove all custom platform code
- Hybrid renderer/handler approach

## Key Dependencies Analysis

### Core MAUI Packages
- **Microsoft.Maui.Controls**: ✅ Already included, latest stable
- **Microsoft.Maui.Essentials**: ✅ Already included, provides cross-platform APIs
- **CommunityToolkit.Maui**: ✅ Already included, essential MAUI extensions

### Platform-Specific Packages
- **Android**: Using AndroidX packages, need verification of MAUI compatibility
- **iOS**: Using standard iOS frameworks through MAUI
- **Windows**: Using Microsoft.WindowsAppSDK for Windows-specific features

### Development Tools
- **Static Analysis**: Roslynator and SonarAnalyzer already configured at solution level
- **Build Tools**: MAUI workload required for development
- **Testing**: MSTest framework recommended for .NET projects

## Performance Considerations

### Build Performance
- Single-project structure should improve build times vs multi-project Xamarin.Forms
- Conditional compilation reduces per-platform overhead
- Package references are already optimized

### Runtime Performance  
- MAUI has performance improvements over Xamarin.Forms
- Platform handlers are more efficient than renderers
- Need benchmarks to validate no regression

### Memory Usage
- MAUI generally has better memory management
- Need to validate no memory leaks in platform-specific code
- Consider impact of CommunityToolkit packages

## Migration Risk Assessment

### Low Risk
- Core project structure is already MAUI
- Main packages are already MAUI-compliant
- Platform targeting is appropriate

### Medium Risk  
- Code in _skip folder may have undocumented dependencies
- Custom effects need individual evaluation
- Some packages may need version updates

### High Risk
- Breaking API changes in consumer applications
- Platform-specific behavior differences
- Test coverage gaps during migration

## Next Steps for Phase 1

1. **Audit _skip folder contents** - Determine what needs migration vs removal
2. **Create test project structure** - Establish testing foundation
3. **Validate package compatibility** - Ensure all dependencies are MAUI-ready
4. **Document API changes** - Identify any breaking changes for consumers
5. **Set up CI/CD validation** - Automated testing for all platforms