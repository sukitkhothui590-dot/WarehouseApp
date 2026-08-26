# Work Summary — C# Warehouse Management Coding Test

## Delivered

- Created CodingTest.sln with two separate .NET 9 projects.
- Implemented WarehouseApp as ASP.NET Core MVC with Razor Views, Bootstrap, EF Core, and SQLite.
- Implemented product management, inventory search/status, receive stock, withdraw stock, dashboard metrics, transaction filters, and product detail history.
- Centralized stock rules in InventoryService.
- Wrapped product quantity updates and transaction inserts in one serializable database transaction.
- Added server-side and browser-side validation, anti-forgery protection, friendly UI errors, and ILogger-based unexpected-error logging.
- Added EF Core InitialCreate migration and five seeded products.
- Implemented RuntimeErrorDemo with an intentional FormatException for non-numeric input.
- Added root README and .gitignore.

## Verification Evidence

- Local .NET SDK installed and used: 9.0.317.
- dotnet restore outputs/CodingTest.sln: passed.
- dotnet build outputs/CodingTest.sln: passed with 0 warnings and 0 errors.
- EF Core migration applied on application startup; InitialCreate listed by dotnet ef migrations list.
- Warehouse app started at http://127.0.0.1:5085; startup log confirmed SQLite migration and seed inserts.
- HTTP/form workflow passed:
  - Seeded P001 visible.
  - Created P100 with opening stock 0.
  - Received 10: stock became 10 and IN history was created.
  - Withdrew 4: stock became 6 and OUT history was created.
  - Withdrew 10: rejected with insufficient-stock feedback; no invalid history was created.
  - Quantity 0 and -1 rejected.
  - Missing product rejected.
  - Duplicate P100 rejected.
  - Product detail and transaction pages showed the expected history.
- RuntimeErrorDemo built successfully and, with input ABC, exited with the expected System.FormatException.

## Files and Projects

- WarehouseApp/: MVC application, models, services, migrations, viewmodels, views, and styles.
- RuntimeErrorDemo/: intentional runtime error console project.
- CodingTest.sln: solution file.
- README.md: setup, feature, business-rule, testing, and runtime-error documentation.
- PROJECT-NOTES.md: Thai development report covering implementation, issues, fixes, design rationale, AI usage, and verification.
- IMAGE-SOURCES.md: local product image source pages and redistribution notes.
- .gitignore: ignores build output, IDE files, and local SQLite files.

## Remaining Limitations

- Browser connector startup failed in this environment, so verification used real local HTTP requests with anti-forgery tokens instead of visual browser automation. The application was still started and exercised against its actual SQLite database.
- Authentication was intentionally omitted because it is outside the stated coding-test requirements.
- Concurrency protection is appropriate for SQLite/local review but is not an enterprise distributed-locking design.

## UX/UI Refresh

- Reworked the application shell to a light B2B admin layout inspired by the supplied reference image: white sidebar, compact topbar, neutral canvas, purple accent, restrained borders, and compact table density.
- Added active navigation states, inventory sub-navigation, profile/status elements, search/focus states, hover feedback, responsive layout, dismissible alerts, and product row selection interaction.
- Reworked the Products screen with toolbar search, filter shortcut, tabs, onboarding/info card, compact product table, status badges, and table footer.
- Preserved all existing warehouse business logic, routes, validation, database schema, and transaction behavior.
- UX refresh verification: full solution build passed with 0 warnings and 0 errors; WarehouseApp restarted on http://127.0.0.1:5085; Products returned HTTP 200 and rendered the new toolbar and overrides stylesheet.

## Product Images

- Added five local product image assets for seeded products P001-P005 under `WarehouseApp/wwwroot/images/products/`.
- Added a local `default-product.svg` fallback for products without a mapped image.
- Product images now render in Products, Inventory, and Product Details views without relying on remote URLs at runtime.
- Documented image source pages and commercial redistribution caveat in `IMAGE-SOURCES.md`.
- Image verification passed: all six local assets returned HTTP 200 from the running app; Products, Inventory, and Product Details each returned HTTP 200 with image markup.

## UX/UI Completion Pass

- Connected the topbar product search to the Products endpoint.
- Added real Products stock-status filtering for In stock, Low stock, and Out of stock.
- Removed the unused product selection checkboxes and exposed direct Edit/Details actions instead.
- Added live Receive/Withdraw movement preview with product image, current balance, projected balance, unit, and insufficient-stock feedback.
- Added client-side submit guarding and confirmation while preserving server-side validation and atomic stock transactions.
- Added product imagery to the Dashboard low-stock list and improved empty-state messaging and responsive movement layout.
- Verification after this pass: solution build passed with 0 warnings and 0 errors; Dashboard, Products search/filter, Receive, Withdraw, and local image endpoints returned HTTP 200; invalid withdraw quantity returned validation feedback while preserving the movement preview.

## Next Steps

1. Clone/open the outputs directory as the repository root.
2. Run dotnet restore and dotnet build.
3. Run dotnet run --project WarehouseApp and open the URL printed by ASP.NET Core.

## Repository Handoff

- Published to https://github.com/sukitkhothui590-dot/WarehouseApp on branch `main`.
- Handoff commits: `47624ab` (`feat: complete warehouse management coding test`), `059645c` (`docs: record repository handoff`), and `b3a9c13` (`docs: add Thai setup and development notes`).
- Remote verification confirmed the repository contains the solution, README, both projects, migrations, styles, JavaScript, and local product images.
- Git status is clean and ignored build/database artifacts are not tracked.
