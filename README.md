# BÀI TẬP LÝ THUYẾT C#

> **Họ và tên:** Hoàng Gia Hưng
> **Mã số sinh viên:** 24810320102

---

## I. PHẦN LÝ THUYẾT & CÂU HỎI NGẮN

### CÂU HỎI

* **Câu 1:** Trình bày sự khác nhau giữa Value Types (Kiểu giá trị) và Reference Types (Kiểu tham chiếu) trong C# về cơ chế lưu trữ vùng nhớ (Stack vs Heap).
* **Câu 2:** Tính năng Init-only Properties (init) trong C# 9/10 khác gì so với thuộc tính có set thông thường? Nêu trường hợp sử dụng thực tế.
* **Câu 3:** Phân biệt sự khác nhau giữa phương thức virtual ở lớp cha và phương thức override ở lớp con khi triển khai tính Đa hình (Polymorphism).
* **Câu 4:** Tại sao một thành phần được khai báo là static trong Lớp (Class) lại không thể truy xuất thông qua một thể hiện (Object Instance) được tạo bằng toán tử new?

---

### BÀI LÀM

#### Câu 1: Phân biệt Value Types và Reference Types về cơ chế lưu trữ vùng nhớ

| Tiêu chí | Value Types (Kiểu giá trị) | Reference Types (Kiểu tham chiếu) |
| :--- | :--- | :--- |
| **Các kiểu đại diện** | `int`, `float`, `bool`, `struct`, `enum`, ... | `string`, `object`, `class`, `interface`, `delegate`, `array`, ... |
| **Cơ chế lưu trữ** | Biến chứa **trực tiếp dữ liệu/giá trị**. Được lưu trên **Stack** (khi là biến cục bộ trong phương thức). Nếu thuộc về một `class`, nó nằm trên **Heap** cùng với class đó. | Biến chỉ chứa **địa chỉ con trỏ (tham chiếu)**. Bản thân đối tượng dữ liệu thực sự luôn được cấp phát và lưu trữ trên **Heap**. |
| **Gán & Sao chép** | Tạo ra một **bản sao dữ liệu độc lập**. Thay đổi biến này hoàn toàn không ảnh hưởng đến biến kia. | Sao chép **địa chỉ tham chiếu**. Cả 2 biến cùng trỏ về một vùng nhớ trên Heap; thay đổi qua biến này sẽ đổi luôn giá trị của biến kia. |
| **Quản lý vùng nhớ** | Tự động giải phóng ngay khi ra khỏi phạm vi (scope) của phương thức. | Được quản lý và dọn dẹp tự động bởi bộ gom rác **Garbage Collector (GC)**. |

---

#### Câu 2: Init-only Properties (`init`) vs Normal `set`

* **Sự khác nhau:**
  * **Normal `set`:** Cho phép gán hoặc thay đổi lại giá trị của thuộc tính ở **bất kỳ thời điểm nào** trong suốt vòng đời của đối tượng.
  * **Init-only (`init`):** Chỉ cho phép gán giá trị **khi khởi tạo đối tượng** (thông qua Constructor hoặc Object Initializer). Sau khi khởi tạo xong, thuộc tính sẽ trở thành Read-Only (không thể sửa đổi).

* **Trường hợp sử dụng thực tế:**
  * Sử dụng khi muốn thiết kế các **đối tượng bất biến (Immutable Objects)**, đảm bảo dữ liệu không bị thay đổi ngoài ý muốn sau khi đã tạo xong.
  * Thường dùng cho: `Id` của người dùng, đối tượng cấu hình hệ thống (`AppConfig`), hoặc các lớp DTO truyền nhận dữ liệu giữa các tầng (Layer) trong ứng dụng.

---

#### Câu 3: Phân biệt `virtual` (Lớp cha) và `override` (Lớp con)

* **`virtual` (Lớp cha):** 
  * Khai báo một phương thức ở lớp cha mà các lớp con có quyền ghi đè.
  * Lớp cha **bắt buộc phải có sẵn phần cài đặt mặc định** (default implementation) cho phương thức này.

* **`override` (Lớp con):** 
  * Được khai báo ở lớp con để **thay thế hoàn toàn phần cài đặt mặc định của lớp cha** bằng logic xử lý riêng của lớp con.

* **Vai trò trong tính Đa hình (Polymorphism):** 
  * Khi gọi một phương thức qua biến kiểu lớp cha nhưng tham chiếu đến đối tượng lớp con, C# Runtime sẽ kiểm tra bảng phương thức ảo (Virtual Method Table) và thực thi phiên bản `override` ở lớp con thay vì phương thức `virtual` ở lớp cha.

---

#### Câu 4: Tại sao thành phần `static` không thể truy xuất qua một Instance?

* **Về mặt quản lý vùng nhớ:** 
  * Thành phần `static` (biến, phương thức) thuộc về **bản thân Lớp (Class)** chứ không thuộc về bất kỳ thể hiện (Instance) cụ thể nào.
  * Vùng nhớ cho thành phần `static` được cấp phát duy nhất một lần khi Class được tải vào bộ nhớ. Trong khi đó, mỗi Instance tạo bằng `new` chỉ quản lý dữ liệu riêng biệt (Instance Members) của bản thân nó.

* **Về mặt thiết kế ngôn ngữ:** 
  * C# cố tình ngăn cản việc gọi `instance.StaticMember` để **tránh gây hiểu nhầm về mặt logic**: giúp người lập trình nhận biết rõ ràng đây là dữ liệu/hành vi dùng chung cho toàn hệ thống, chứ không bị phụ thuộc hay thay đổi theo từng đối tượng cụ thể.
