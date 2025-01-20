using System;

namespace Lombiq.DataTables.Tests.UI;

[Flags]
public enum TestDataTableRecipeDataSections
{
    None = 0,
    MainMenu = 1,
    TagHelper = 1 << 2,
    ProviderWithShape = 1 << 3,
    JsonBasedProvider = 1 << 4,
    IndexBasedProvider = 1 << 5,
    All = MainMenu | TagHelper | ProviderWithShape | JsonBasedProvider | IndexBasedProvider,
}
