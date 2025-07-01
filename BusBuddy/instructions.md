# Instructions for Using Syncfusion in BusBuddy (.NET MAUI)

This project is set up to use Syncfusion controls and libraries for all UI and component needs. Follow these guidelines to ensure consistency and best practices:

## 1. UI Components
- **Always use Syncfusion MAUI controls** (e.g., `SfTabView`, `SfListView`, `SfDataGrid`, etc.) for all new UI features.
- Do **not** use Microsoft.Maui.Controls UI elements (such as `Button`, `Entry`, `ListView`, etc.) unless there is no Syncfusion equivalent.
- Reference Syncfusion controls in XAML using the appropriate namespace, for example:
  ```xml
  xmlns:syncfusion="clr-namespace:Syncfusion.Maui.TabView;assembly=Syncfusion.Maui.TabView"
  ```

## 2. Navigation
- Use the standard .NET MAUI `Shell` for navigation, but place Syncfusion controls inside your pages.
- Do not attempt to replace `Shell` with a Syncfusion control, as there is no direct equivalent.

## 3. Licensing
- Register your Syncfusion license in `App.xaml.cs` using:
  ```csharp
  SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY");
  ```
- The license key can be set via environment variable `SYNCFUSION_LICENSE_KEY`.

## 4. Adding New Features
- When adding new pages or features, always check for a Syncfusion control before using a default MAUI control.
- If a Syncfusion control does not exist for your use case, document the reason for using a default control in your code.

## 5. Resources
- Refer to the [Syncfusion MAUI documentation](https://help.syncfusion.com/maui/introduction/overview) for usage examples and API references.

## 6. Support
- For issues with Syncfusion controls, consult the official documentation or Syncfusion support.

---

**Summary:**
- Use only Syncfusion controls for UI and data presentation.
- Use .NET MAUI Shell for navigation structure.
- Register and manage Syncfusion licenses as shown in `App.xaml.cs`.
- Document any exceptions to these rules in your code.
