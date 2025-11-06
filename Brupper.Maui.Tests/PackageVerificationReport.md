# Package Verification Report

**Date**: November 6, 2025
**Status**: ⚠️ **BLOCKED - Package Conflicts**

## Summary

Package verification is currently blocked by conflicts between centrally managed package versions in `Directory.Packages.props` and transitive dependency requirements from `Xamarin.Google.Android.Material`.

## Issues Found

### 1. Package Version Conflicts (CRITICAL)

The following package version downgrades are being reported:

| Package | Required Version | Centrally Defined | Source |
|---------|------------------|-------------------|--------|
| Xamarin.AndroidX.AppCompat | 1.7.0.5 | 1.7.0.4 | Xamarin.Google.Android.Material 1.12.0.2 |
| Xamarin.AndroidX.CardView | 1.0.0.33 | 1.0.0.17 | Xamarin.Google.Android.Material 1.12.0.2 |
| Xamarin.AndroidX.RecyclerView | 1.3.2.10 | 1.2.1.8 | Xamarin.Google.Android.Material 1.12.0.2 |
| Xamarin.AndroidX.ViewPager | 1.1.0.1 | 1.0.0.15 | Xamarin.AndroidX.Fragment 1.8.5.2 |

**Root Cause**: `Xamarin.Google.Android.Material` version 1.12.0.2 is defined in `Directory.Packages.props` line 235 and is pulling in higher versions of AndroidX packages as transitive dependencies than what is centrally defined.

### 2. Central Package Management Issues

The repository uses Central Package Management (CPM) which requires all package versions to be defined in `Directory.Packages.props`. However, there are:

- Duplicate PackageVersion items for `Microsoft.CodeAnalysis.Analyzers` (was: 3.11.0, now consolidated to 3.11.0)
- Duplicate PackageVersion items for `Microsoft.CodeAnalysis.CSharp.Workspaces` (was: 4.12.0 and 4.13.0, now consolidated to 4.13.0)
- Outdated AndroidX package versions that conflict with newer transitive dependencies

### 3. MAUI Package Status

**MAUI-Compatible Packages** (✅ Active in project):
- `Microsoft.Maui.Essentials` - 9.0.40
- `Microsoft.Maui.Controls` - 9.0.40
- `Microsoft.AppCenter.Analytics` - 5.0.7
- `Microsoft.AppCenter.Crashes` - 5.0.7
- `SkiaSharp.Svg` - 1.60.0
- `SkiaSharp.Views.Maui.Controls.Compatibility` - 2.88.9

**MAUI-Compatible Packages** (⚠️ Currently Commented Out - Need to Re-enable):
- `CommunityToolkit.Maui` - 11.1.0
- `CommunityToolkit.Maui.Markup` - 5.1.0
- `CommunityToolkit.Mvvm` - 8.4.0

**Legacy Xamarin Packages** (❌ Commented Out - Good):
- `Xamarin.AndroidX.AppCompat`
- `Xamarin.AndroidX.Fragment`
- `Xamarin.AndroidX.Preference`
- `Xamarin.AndroidX.ViewPager`
- `Xamarin.Google.Android.Material`
- `Xamarin.CommunityToolkit`
- `Xamarin.FFImageLoading.*`

## Resolution Strategy

### Option 1: Update Central Package Versions (RECOMMENDED)

Update `Directory.Packages.props` to use the higher versions required by transitive dependencies:

```xml
<PackageVersion Include="Xamarin.AndroidX.AppCompat" Version="1.7.0.5" />
<PackageVersion Include="Xamarin.AndroidX.AppCompat.AppCompatResources" Version="1.7.0.5" />
<PackageVersion Include="Xamarin.AndroidX.CardView" Version="1.0.0.33" />
<PackageVersion Include="Xamarin.AndroidX.RecyclerView" Version="1.3.2.10" />
<PackageVersion Include="Xamarin.AndroidX.ViewPager" Version="1.1.0.1" />
```

**Status**: ✅ Already attempted and updated in Directory.Packages.props
**Issue**: The file seems to be getting reset or there's a caching issue preventing the changes from taking effect.

### Option 2: Remove Xamarin Packages from Central Package Management

Since this is a MAUI migration, all Xamarin packages should eventually be removed. However, other projects in the solution may still depend on them.

**Action Required**: Audit all projects in the solution to determine which ones still need Xamarin packages.

### Option 3: Temporarily Disable Central Package Management for Brupper.Maui

Add to Brupper.Maui.csproj:
```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

**Risk**: This breaks consistency with the rest of the solution.

## Next Steps

1. ✅ **COMPLETED**: Document package conflicts
2. ⏭️ **NEXT**: Investigate why Directory.Packages.props changes aren't persisting
3. ⏭️ **FUTURE**: Re-enable CommunityToolkit packages after conflicts are resolved
4. ⏭️ **FUTURE**: Remove all Xamarin package dependencies from Directory.Packages.props
5. ⏭️ **FUTURE**: Verify project builds successfully for all target frameworks

## Recommendations

1. **Immediate**: Focus on completing other setup tasks while package resolution is investigated
2. **Short-term**: Coordinate with team to update Directory.Packages.props with correct versions
3. **Long-term**: Plan migration away from all Xamarin packages across the entire solution

## Impact on Migration

**Migration Status**: ⏸️ **PAUSED ON T007**

This package conflict is blocking:
- T007: Verify all package references in Maui/Brupper.Maui.csproj are MAUI-compatible
- T008: Update Microsoft.Extensions.DependencyInjection integration (dependent on package resolution)

**Workaround**: Continue with other independent tasks:
- T005: ✅ Audit legacy code (COMPLETED)
- T006: ✅ Create migration tracking (COMPLETED)
- T009-T012: Foundational tasks (CAN PROCEED - marked as [P] parallel)

## Test Project Status

The test project (`Brupper.Maui.Tests`) is configured and working correctly:
- ✅ xUnit framework configured
- ✅ Test project builds successfully
- ✅ Performance baseline tests created and passing
- ⚠️ Project reference to Brupper.Maui temporarily commented out due to package conflicts