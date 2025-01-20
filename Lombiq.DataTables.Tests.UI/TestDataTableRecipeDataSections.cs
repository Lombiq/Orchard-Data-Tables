using System;

namespace Lombiq.DataTables.Tests.UI;

[Flags]
public enum TestDataTableRecipeDataSections
{
    None = 0,
    MainMenu = 1,
    TagHelper = 1 << 2,
    ProviderWithShape = 1 << 3,
    IndexBasedProvider = 1 << 4,
    All = MainMenu | TagHelper | ProviderWithShape | IndexBasedProvider,
}
