# Coding Test — Warehouse Management System

โปรเจกต์นี้เป็น Coding Test ภาษา C# ประกอบด้วยระบบจัดการคลังสินค้า และโปรเจกต์ตัวอย่างที่ Compile ผ่านแต่เกิด Runtime Error ตามโจทย์

รายละเอียดการพัฒนา ปัญหาที่พบ วิธีแก้ เหตุผลทางเทคนิค และการใช้ AI ดูได้ที่ [PROJECT-NOTES.md](PROJECT-NOTES.md)

## โปรเจกต์ภายใน Repository

### 1. WarehouseApp

เว็บแอปพลิเคชันสำหรับจัดการสินค้า รับสินค้าเข้า เบิกสินค้าออก ดูยอดคงเหลือ และตรวจสอบประวัติการเคลื่อนไหวของสินค้า

### 2. RuntimeErrorDemo

โปรแกรม Console ที่ Compile ผ่านและทำงานปกติเมื่อกรอกตัวเลข แต่ตั้งใจให้เกิด `System.FormatException` เมื่อกรอกข้อความ เช่น `ABC`

## เทคโนโลยี

- C# / .NET 9
- ASP.NET Core MVC และ Razor Views
- Entity Framework Core 9
- SQLite
- Bootstrap, CSS และ Vanilla JavaScript

## สิ่งที่ต้องติดตั้ง

- .NET 9 SDK หรือใหม่กว่า
- ไม่ต้องติดตั้ง Database Server เพิ่ม

ตรวจสอบ .NET SDK:

```bash
dotnet --version
```

## วิธีติดตั้งและใช้งาน WarehouseApp

เปิด Terminal แล้วรันคำสั่ง:

```bash
git clone https://github.com/sukitkhothui590-dot/WarehouseApp.git
cd WarehouseApp
dotnet restore
dotnet build
dotnet run --project WarehouseApp/WarehouseApp.csproj
```

จากนั้นเปิดเว็บตาม URL ที่แสดงใน Terminal โดยค่าเริ่มต้นของโปรเจกต์คือ:

```text
http://localhost:5036
```

ระบบจะสร้าง SQLite database, รัน EF Core migration และเพิ่มข้อมูลตัวอย่างให้อัตโนมัติเมื่อเปิดครั้งแรก

ไฟล์ฐานข้อมูลจะถูกสร้างที่:

```text
WarehouseApp/warehouse.db
```

ถ้าต้องการรันจากโฟลเดอร์โปรเจกต์โดยตรง:

```bash
cd WarehouseApp
dotnet run
```

## วิธีใช้งานหลัก

1. เปิดหน้า Dashboard เพื่อดูภาพรวมสินค้าและการเคลื่อนไหวล่าสุด
2. ไปที่ Products เพื่อดู เพิ่ม หรือแก้ไขข้อมูลสินค้า
3. ใช้ Receive stock เพื่อรับสินค้าเข้าคลัง
4. ใช้ Withdraw stock เพื่อเบิกสินค้าออกจากคลัง
5. ใช้ Inventory เพื่อค้นหาสินค้าและดูสถานะ In stock, Low stock หรือ Out of stock
6. ใช้ Transactions เพื่อตรวจสอบประวัติ Receive/Withdraw
7. เปิด Product Details เพื่อดูข้อมูลสินค้าและประวัติของสินค้านั้นโดยเฉพาะ

หน้า Receive/Withdraw มี movement preview แสดงยอดก่อนทำรายการและยอดหลังทำรายการ พร้อมป้องกันการเบิกเกิน stock ก่อนส่งข้อมูล

## วิธีรัน RuntimeErrorDemo

จาก root ของ repository:

```bash
dotnet run --project RuntimeErrorDemo/RuntimeErrorDemo.csproj
```

ทดลองกรอก:

```text
10
```

โปรแกรมจะทำงานปกติ จากนั้นทดลองกรอก:

```text
ABC
```

ผลลัพธ์ที่คาดหวังคือ `System.FormatException` ซึ่งเป็น Runtime Error ที่ตั้งใจสร้างตามโจทย์

## ฟีเจอร์ที่ทำไว้

- Dashboard metrics: จำนวนสินค้า จำนวนหน่วยคงเหลือ รับเข้า/เบิกออกวันนี้ สินค้าใกล้หมด และรายการล่าสุด
- เพิ่ม แก้ไข ดูรายการ และดูรายละเอียดสินค้า
- รับสินค้าเข้าและเบิกสินค้าออกพร้อมบันทึกประวัติ
- ตรวจสอบและป้องกัน stock ติดลบ
- ค้นหาสินค้าด้วยรหัสหรือชื่อ
- Filter สินค้าตามสถานะ stock
- Filter ประวัติธุรกรรมตามประเภทและสินค้า
- Validation ฝั่ง Server และ Client
- Success/Error feedback ที่อ่านง่าย
- รูปสินค้า local พร้อม fallback image
- Responsive business/admin UI สำหรับ Desktop และ Tablet

## โครงสร้างโปรเจกต์

```text
CodingTest/
├── WarehouseApp/
│   ├── Controllers/
│   ├── Data/Migrations/
│   ├── Models/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   └── wwwroot/
├── RuntimeErrorDemo/
├── CodingTest.sln
├── README.md
├── PROJECT-NOTES.md
└── IMAGE-SOURCES.md
```

## Business Rules สำคัญ

- Product code ต้องไม่ซ้ำ
- ชื่อและหน่วยสินค้าห้ามว่าง
- สินค้าใหม่เริ่มต้นด้วย stock 0
- Receive/Withdraw ต้องมีจำนวนมากกว่า 0
- Withdraw ห้ามเกินจำนวน stock ปัจจุบัน
- ทุก stock movement ต้องบันทึก `BalanceBefore`, `BalanceAfter`, ประเภท จำนวน หมายเหตุ และเวลา
- การ update stock และเพิ่ม transaction history อยู่ใน Database Transaction เดียวกัน
- ไม่มี Delete Product เพื่อป้องกันประวัติธุรกรรมเสียความสัมพันธ์

## การทดสอบที่แนะนำ

```bash
dotnet restore
dotnet build CodingTest.sln
```

ทดสอบ WarehouseApp:

1. สร้าง `P100 / Test Product / pcs` และตรวจสอบว่า stock เป็น 0
2. Receive จำนวน 10 ต้องได้ stock 10 และ transaction `IN`
3. Withdraw จำนวน 4 ต้องได้ stock 6 และ transaction `OUT`
4. Withdraw จำนวน 10 ต้องถูกปฏิเสธและ stock ต้องยังเป็น 6
5. ทดสอบจำนวน 0 และ -1 ต้องถูกปฏิเสธ
6. สร้าง `P100` ซ้ำ ต้องถูกปฏิเสธ
7. ตรวจสอบ Dashboard, Inventory, Product Details และ Transactions

## ฐานข้อมูล

ใช้ SQLite เพื่อให้ผู้ตรวจสามารถ clone แล้ว run ได้ทันทีโดยไม่ต้องติดตั้ง SQL Server หรือ database server อื่น โดย EF Core migration จะทำงานอัตโนมัติเมื่อเริ่มแอป

## รูปสินค้า

รูปสินค้าถูกเก็บไว้ใน `WarehouseApp/wwwroot/images/products/` และทำงานได้แม้ไม่มี Internet ขณะรัน ระบบจะใช้ `default-product.svg` หากไม่มีรูปที่ map ไว้ ดู source pages ได้ที่ [IMAGE-SOURCES.md](IMAGE-SOURCES.md)

## หมายเหตุสำหรับผู้ตรวจ

- ระบบนี้ตั้งใจไม่ทำ Authentication เพราะไม่อยู่ใน requirement หลัก
- `RuntimeErrorDemo` ตั้งใจให้เกิด Runtime Error เมื่อกรอก `ABC` ห้ามแก้เป็น `TryParse` หากต้องการตรวจตามโจทย์
- ดูบันทึกการพัฒนาและผลการตรวจสอบได้ที่ [PROJECT-NOTES.md](PROJECT-NOTES.md)
