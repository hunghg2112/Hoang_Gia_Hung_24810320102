I. PHẦN LÝ THUYẾT & CÂU HỎI NGẮN
Câu 1: Phân biệt Value Types và Reference Types trong C#
Value Types (kiểu giá trị) và Reference Types (kiểu tham chiếu) khác nhau chủ yếu ở cách biến chứa dữ liệu.

Value Type: biến chứa trực tiếp giá trị.

Ví dụ: int, double, bool, struct, enum.

Khi gán một biến Value Type cho biến khác, giá trị được sao chép.

Thông thường, biến cục bộ có thể nằm trên Stack, nhưng không nên hiểu rằng mọi Value Type luôn nằm trên Stack; vị trí thực tế phụ thuộc vào ngữ cảnh và cách CLR quản lý bộ nhớ.

Reference Type: biến chứa tham chiếu (reference) đến đối tượng được lưu trong Heap.

Ví dụ: class, string, array, delegate.

Khi gán một biến Reference Type cho biến khác, tham chiếu được sao chép, nên hai biến có thể cùng trỏ đến một đối tượng.

Ví dụ:

int a = 10;
int b = a;
b = 20;

// a vẫn bằng 10

class Student
{
    public string Name;
}

Student s1 = new Student();
s1.Name = "An";

Student s2 = s1;
s2.Name = "Binh";

// s1.Name cũng là "Binh"

Lưu ý: Cách nói "Value Type = Stack, Reference Type = Heap" là cách đơn giản hóa. Chính xác hơn là Value Type lưu trực tiếp giá trị, còn Reference Type lưu một tham chiếu đến đối tượng; việc dữ liệu thực tế nằm ở đâu còn phụ thuộc vào ngữ cảnh thực thi.

Câu 2: init khác gì set thông thường?
init được giới thiệu trong C# 9, cho phép thuộc tính chỉ được gán trong quá trình khởi tạo đối tượng, sau đó không thể thay đổi.

set: có thể gán hoặc thay đổi giá trị bất kỳ lúc nào mà setter cho phép.

init: chỉ cho phép gán khi:

Khởi tạo bằng object initializer.

Trong constructor.

Trong một số ngữ cảnh khởi tạo hợp lệ khác.

Ví dụ:

class Student
{
    public string Name { get; init; }
    public int Age { get; set; }
}

Sử dụng:

Student student = new Student
{
    Name = "Nguyen Van A",
    Age = 20
};

student.Age = 21;       // Hợp lệ
student.Name = "B";     // Lỗi biên dịch

Trường hợp thực tế: init phù hợp với các đối tượng mà một số thông tin cần bất biến sau khi khởi tạo, chẳng hạn:

class User
{
    public int Id { get; init; }
    public string Email { get; init; }
    public string DisplayName { get; set; }
}

Id và Email có thể được xác định lúc tạo User và không cho phép thay đổi tùy ý về sau.

Câu 3: virtual và override trong tính đa hình
virtual được khai báo ở lớp cha, cho biết phương thức có thể được lớp con ghi đè.

override được khai báo ở lớp con, dùng để cung cấp cách triển khai mới cho phương thức virtual của lớp cha.

Ví dụ:

class Animal
{
    public virtual void Sound()
    {
        Console.WriteLine("Animal sound");
    }
}

class Dog : Animal
{
    public override void Sound()
    {
        Console.WriteLine("Woof");
    }
}

Khi sử dụng:

Animal animal = new Dog();
animal.Sound();

Kết quả:

Woof

Đây chính là đa hình (Polymorphism): dù biến animal có kiểu Animal, phương thức được thực thi là phiên bản Sound() của Dog.

Có thể hiểu ngắn gọn:

Từ khóa	Được dùng ở	Ý nghĩa
virtual	Lớp cha	Cho phép lớp con ghi đè
override	Lớp con	Ghi đè phương thức của lớp cha

Câu 4: Tại sao static không truy xuất thông qua Object Instance?
Thành phần static thuộc về chính lớp (Class), không thuộc về từng đối tượng (Object Instance).

Ví dụ:

class Student
{
    public static int Count = 0;
}

Count chỉ có một bản sao dùng chung cho toàn bộ lớp Student, thay vì mỗi object Student có một Count riêng.

Vì vậy, cách truy xuất đúng là:

Student.Count++;

Không phải:

Student s = new Student();
s.Count++;       // Không được truy xuất static theo cách này

Lý do là khi tạo:

Student s = new Student();

new tạo ra một instance của Student, nhưng static Count không thuộc instance đó. Nó thuộc về type Student.

Có thể ghi nhớ:

Instance member → truy xuất qua Object.
Static member → truy xuất qua Class.
