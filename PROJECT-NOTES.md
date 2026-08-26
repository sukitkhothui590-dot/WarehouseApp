# บันทึกการพัฒนา — Warehouse Management Coding Test

เอกสารนี้สรุปสิ่งที่พัฒนา เหตุผลของแนวทาง ปัญหาที่พบ วิธีแก้ และบทบาทของ AI ในงานนี้ เพื่อให้ผู้ตรวจเห็นภาพรวมของกระบวนการทำงานได้ชัดเจน

## 1. เป้าหมายของงาน

เป้าหมายคือสร้างระบบคลังสินค้าที่ใช้งานได้จริง ไม่ใช่เพียงหน้า Mockup โดยต้องมี Database, Business Logic, Validation, Transaction History, UI สำหรับ Admin และโปรเจกต์ C# ที่ Compile ผ่านแต่สามารถแสดง Runtime Error ได้ตามโจทย์

## 2. สิ่งที่พัฒนา

### WarehouseApp

- สร้าง ASP.NET Core MVC Web Application ด้วย C# และ Razor Views
- สร้าง Product และ StockTransaction Entity พร้อม Foreign Key แบบ Product 1 ต่อ Many Transactions
- ใช้ EF Core Migration และ SQLite
- สร้าง Service Layer ชื่อ `InventoryService` เพื่อรวมกฎการรับ/เบิกสินค้าไว้ที่เดียว
- เพิ่ม Dashboard แสดงจำนวนสินค้า ยอด stock รับเข้า/เบิกออกวันนี้ สินค้าใกล้หมด และรายการล่าสุด
- เพิ่ม Product Management: Create, View, Edit และ Product Details
- เพิ่ม Receive Stock และ Withdraw Stock
- เพิ่ม Inventory Search และ Status Filter
- เพิ่ม Transaction History พร้อม filter ประเภทและสินค้า
- เพิ่ม server-side validation, client-side validation, anti-forgery token และ error feedback
- ทำให้การแก้ยอดสินค้าและเพิ่มประวัติอยู่ใน Database Transaction เดียวกัน
- เพิ่ม local product images, fallback image และ source document
- ปรับ UI ให้เป็น Business/Admin Dashboard โทนเรียบง่าย เป็นทางการ และมี accent สีม่วงแบบ modern
- เพิ่ม global search, stock filter, live movement preview, confirmation และ responsive layout

### RuntimeErrorDemo

- แยกเป็น Console Project ตามโจทย์
- ใช้ `int.Parse` เพื่อให้ input ที่เป็นตัวเลขทำงานปกติ
- เมื่อกรอก `ABC` จะเกิด `System.FormatException` โดยตั้งใจ

## 3. เหตุผลที่เลือกแนวทางนี้

### ASP.NET Core MVC + Razor

โจทย์เน้น C#, Database และ Business Logic จึงเลือก MVC แทน SPA Framework เพื่อให้โครงสร้างอ่านง่ายและผู้ตรวจสามารถไล่ดู Controller, Service, Model และ View ได้โดยตรง

### SQLite

SQLite ทำให้ผู้ตรวจ clone แล้ว run ได้ทันทีโดยไม่ต้องติดตั้ง SQL Server หรือเปิด Database Server ภายนอก เหมาะกับ Coding Test ที่ต้องส่งให้ทดลองอย่างรวดเร็ว

### Entity Framework Core

EF Core ช่วยให้ Schema, Relationship และ Migration อยู่ใน Source Code สามารถสร้าง Database ใหม่จาก Migration ได้อย่างสม่ำเสมอ

### Service Layer

กฎสำคัญ เช่น ห้าม stock ติดลบ ตรวจสอบจำนวน และสร้าง transaction history ไม่ควรกระจายอยู่ใน Controller จึงรวมไว้ใน `InventoryService` เพื่อให้อ่านง่าย ทดสอบง่าย และลดโอกาสที่ logic จะไม่ตรงกัน

### Database Transaction

การเปลี่ยนยอดสินค้าและการบันทึกประวัติต้องสำเร็จหรือ rollback พร้อมกัน ป้องกันกรณี stock เปลี่ยนแต่ไม่มีประวัติ หรือมีประวัติแต่ยอดสินค้าไม่เปลี่ยน

### Vanilla JavaScript + CSS

ใช้ JavaScript และ CSS ที่มีอยู่ในโปรเจกต์แทนการเพิ่ม dependency ใหม่ เพื่อรักษา bundle ให้เล็กและเหมาะกับขนาดของ Coding Test

## 4. ปัญหาที่พบและวิธีแก้

### ปัญหา: ต้องรักษาความสัมพันธ์ของ stock กับ transaction

ถ้า update Product ก่อนแล้วค่อยสร้าง Transaction แยกกัน อาจเกิดข้อมูลไม่ตรงกันเมื่อมี error ระหว่างทาง

วิธีแก้: ใช้ EF Core Database Transaction ครอบขั้นตอนอ่านยอด ตรวจสอบ แก้ยอด เพิ่มประวัติ และ SaveChanges ให้เป็น operation เดียวกัน ถ้าเกิดข้อผิดพลาดจะ rollback

### ปัญหา: เบิกสินค้าเกินจำนวนคงเหลือ

วิธีแก้: ตรวจสอบที่ Service Layer ซึ่งเป็นจุดบังคับใช้จริง และเพิ่ม preview ฝั่ง UI เพื่อแจ้งเตือนก่อน submit อีกชั้นหนึ่ง โดยยังคงให้ Server เป็นผู้ตัดสินผลสุดท้าย

### ปัญหา: Product Code ซ้ำ

วิธีแก้: ตรวจสอบซ้ำทั้งตอน Create และ Edit ก่อนบันทึก พร้อมแสดงข้อความ validation ที่ field Code

### ปัญหา: หน้า UI มี interaction ที่ยังไม่สมบูรณ์

เดิม global search ยังไม่ submit จริง, ปุ่ม filter ยังไม่กรองสถานะ และ checkbox ไม่มี action ต่อเนื่อง

วิธีแก้: เชื่อม global search กับ Products endpoint, เพิ่ม filter In stock/Low stock/Out of stock และเปลี่ยน action ในตารางเป็น Edit/Details ที่ใช้งานได้จริง

### ปัญหา: หน้า Receive/Withdraw ไม่เห็นผลลัพธ์ก่อนทำรายการ

วิธีแก้: เพิ่ม movement preview ที่แสดงรูปสินค้า ยอดปัจจุบัน และยอดหลังทำรายการแบบทันทีเมื่อเลือกสินค้า/กรอกจำนวน รวมถึง confirmation ก่อนส่งข้อมูล

### ปัญหา: รูปสินค้าจาก Internet ไม่ควรเป็น dependency ตอนรัน

วิธีแก้: ดาวน์โหลดรูปตัวอย่างมาเก็บ local ใน `wwwroot/images/products/` และเพิ่ม fallback SVG สำหรับสินค้าที่ไม่มีรูป ทั้งนี้ source pages ถูกบันทึกไว้ใน `IMAGE-SOURCES.md` และควรตรวจ license ก่อนนำไปใช้เชิงพาณิชย์

### ปัญหา: Browser automation เชื่อมต่อไม่ได้ใน Environment นี้

วิธีแก้: ใช้การทดสอบกับ Runtime จริงผ่าน HTTP requests, ตรวจ status code, ตรวจ HTML markup, ตรวจการโหลด image assets และส่ง form validation พร้อม anti-forgery token แทน โดยไม่ได้สรุปเกินกว่าหลักฐานที่ตรวจได้

## 5. การนำ AI มาใช้

AI ถูกใช้เป็นผู้ช่วยด้านการวิเคราะห์และพัฒนา ไม่ได้ใช้แทนการตรวจสอบผลลัพธ์ โดยใช้ในขอบเขตต่อไปนี้:

- วิเคราะห์ requirement และแยก feature ที่ต้องทำก่อน เช่น Database, Stock Rules, Validation และ Runtime Verification
- ช่วยวางโครงสร้างโปรเจกต์ MVC, Entity, ViewModel, Service และ Controller
- ช่วยร่างและปรับปรุง UI ให้สอดคล้องกับภาพตัวอย่างที่ได้รับ
- ช่วยตรวจหา edge cases เช่น stock ติดลบ, จำนวน 0, จำนวนติดลบ, Product ไม่พบ และรหัสซ้ำ
- ช่วยวิเคราะห์ error จาก Build/Runtime และเสนอแนวทางแก้แบบ minimal change
- ช่วยสร้าง test checklist และตรวจ workflow รับเข้า/เบิกออก/ประวัติ
- ช่วยจัดทำ README, source notes และเอกสารส่งมอบ
- ใช้ Web/Image search เพื่อหา asset ตัวอย่าง จากนั้นเก็บไฟล์ไว้ local และตรวจว่าระบบโหลดได้จริง

ทุกการเปลี่ยนแปลงถูกตรวจด้วย `dotnet build`, การเปิดแอปจริง, HTTP smoke test และการตรวจผลลัพธ์จาก SQLite/หน้าเว็บก่อนสรุปงาน

## 6. ผลการตรวจสอบ

- `dotnet restore`: ผ่าน
- `dotnet build CodingTest.sln`: ผ่าน 0 warnings และ 0 errors
- EF Core migration และ SQLite initialization: ผ่าน
- WarehouseApp start และ listen ได้จริง
- Dashboard, Products, Inventory, Receive, Withdraw, Transactions และ Product Details: HTTP 200
- Product search และ stock status filter: ทำงานจริง
- Local image assets ทั้งหมด: โหลดได้ HTTP 200
- Receive 10: stock เพิ่มถูกต้องและสร้าง `IN` transaction
- Withdraw 4: stock ลดถูกต้องและสร้าง `OUT` transaction
- Withdraw เกิน stock: ถูกปฏิเสธและไม่สร้าง transaction ผิด
- Quantity 0 และ -1: ถูกปฏิเสธ
- Product Code ซ้ำ: ถูกปฏิเสธ
- RuntimeErrorDemo กรอก `ABC`: เกิด `System.FormatException` ตามโจทย์

## 7. ข้อจำกัดที่ตั้งใจไว้

- ไม่มี Authentication เพราะไม่อยู่ใน requirement หลักของ Coding Test
- ไม่มี Delete Product เพื่อไม่ให้ Transaction History เสียความสัมพันธ์
- รูปสินค้าปัจจุบันเป็น demo assets แบบ map ตาม Product Code ยังไม่มีระบบอัปโหลดรูปสำหรับผู้ใช้
- SQLite เหมาะกับการ review/local run ไม่ใช่การออกแบบ distributed production concurrency ขนาดใหญ่
- การตรวจครั้งนี้ใช้ HTTP/runtime verification แทน browser automation เนื่องจาก connector ของ environment ไม่พร้อมใช้งาน

## 8. วิธีส่งมอบ

Repository ถูกเผยแพร่ไว้ที่:

```text
https://github.com/sukitkhothui590-dot/WarehouseApp
```

ผู้ตรวจสามารถเริ่มจากคำสั่งใน `README.md` ได้ทันที โดยไม่ต้องเตรียม Database Server เพิ่ม
