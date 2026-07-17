# Quy tắc Đánh giá và Hướng dẫn Chi tiết Sơ đồ Lớp (Class Diagram)

Tài liệu này cung cấp định nghĩa chuẩn hóa, các góc nhìn (perspectives), ký pháp thành phần và quy tắc đánh giá chi tiết cho Sơ đồ Lớp (Class Diagram) dựa trên tiêu chuẩn UML.

---

## I. ĐỊNH NGHĨA VÀ THÀNH PHẦN CỦA LỚP (CLASS NOTATION)

Một Lớp (Class) là bản thiết kế (blueprint) đại diện cho một khái niệm trong hệ thống, bao gồm trạng thái (state/attributes) và hành vi (behavior/operations). Ký pháp của Lớp được chia thành 3 phân vùng (partitions) chính từ trên xuống dưới:

1. **Tên Lớp (Class Name)**: 
   - Nằm ở phân vùng đầu tiên. Đây là thông tin bắt buộc duy nhất của lớp.
   - Các lớp trừu tượng (Abstract Class) hoặc Interface phải được viết in nghiêng (italics) hoặc có ký hiệu stereotype tương ứng (ví dụ: `<<interface>>`).

2. **Thuộc tính (Attributes)**:
   - Nằm ở phân vùng thứ hai. Biểu diễn các biến thành viên (member variables/data members) trong mã nguồn.
   - Định dạng hiển thị: `Visibility attributeName : Type` (ví dụ: `- id : int`).

3. **Phương thức/Hành vi (Operations / Methods)**:
   - Nằm ở phân vùng thứ ba. Biểu diễn các dịch vụ/hàm mà lớp cung cấp.
   - Định dạng hiển thị: `Visibility operationName(direction parameterName : ParameterType) : ReturnType` (ví dụ: `+ login() : boolean`).

### Ký hiệu tầm vực (Class Visibility):
- `+` : **Public** - Cho phép truy cập từ mọi lớp khác.
- `-` : **Private** - Chỉ cho phép truy cập nội bộ trong chính lớp đó.
- `#` : **Protected** - Chỉ cho phép truy cập từ nội bộ lớp đó và các lớp con kế thừa từ nó.

### Hướng truyền tham số (Parameter Directionality):
Trước tên tham số trong phương thức có thể ký hiệu hướng truyền dữ liệu so với caller:
- `in` : Tham số truyền vào hàm (chỉ đọc).
- `out` : Tham số dùng để trả dữ liệu ra ngoài cho caller.
- `inout` : Tham số vừa truyền vào vừa cập nhật giá trị trả ra ngoài.

---

## II. CÁC GÓC NHÌN TRONG THIẾT KẾ (PERSPECTIVES OF CLASS DIAGRAM)

Mức độ chi tiết của Sơ đồ Lớp phụ thuộc vào giai đoạn phát triển dự án:
1. **Conceptual Perspective (Góc nhìn Khái niệm)**: Biểu diễn các thực thể/khái niệm thực tế trong thế giới thực của domain nghiệp vụ. Giai đoạn này hiếm khi quan tâm đến kiểu dữ liệu phần mềm hay interface cụ thể.
2. **Specification Perspective (Góc nhìn Đặc tả)**: Tập trung vào giao diện của các kiểu dữ liệu trừu tượng (Abstract Data Types - ADTs) hoặc Interface trong phần mềm, định nghĩa hành vi mà không đi sâu vào code triển khai.
3. **Implementation Perspective (Góc nhìn Triển khai)**: Mô tả chi tiết cách các lớp thực thi các thuộc tính, phương thức và các mối quan hệ cụ thể trong mã nguồn thực tế.

---

## III. CHI TIẾT CÁC MỐI QUAN HỆ GIỮA CÁC LỚP (RELATIONSHIPS)

Sơ đồ Lớp truyền tải chính xác cách lập trình viên hiện thực hóa mã nguồn từ bản vẽ thông qua 6 mối quan hệ cốt lõi sau:

### 1. Kế thừa / Khái quát hóa (Inheritance / Generalization)
- **Ý nghĩa**: Thể hiện mối quan hệ "is-a" (Con là một loại Cha). Lớp con thừa hưởng tất cả thuộc tính, phương thức và các mối quan hệ của lớp cha.
- **Ký pháp**: Đường nét liền với đầu mũi tên hình tam giác rỗng (`--|>`) trỏ từ Con về Cha.
- **Quy tắc**: Mũi tên bắt buộc phải hướng về lớp Cha.

### 2. Hiện thực hóa (Realization)
- **Ý nghĩa**: Mối quan hệ giữa một giao diện đặc tả (Interface) và lớp cụ thể cung cấp mã nguồn thực thi cho giao diện đó.
- **Ký pháp**: Đường nét đứt với đầu mũi tên hình tam giác rỗng (`..|>`) trỏ từ Lớp cụ thể về Interface.

### 3. Liên kết (Association)
- **Ý nghĩa**: Mối quan hệ cấu trúc tĩnh giữa các lớp ngang hàng. Một lớp giữ tham chiếu đến lớp kia dưới dạng thuộc tính (member field) trong thời gian dài. Có thể đi kèm với Lực lượng/Độ đa dạng (Cardinality/Multiplicity) như `1`, `0..*`, `*`.
- **Ký pháp**: Đường nét liền (`--`). Có thể có hướng mũi tên nhọn (`-->`) nếu là liên kết 1 chiều.

### 4. Thu nạp (Aggregation)
- **Ý nghĩa**: Một dạng liên kết đặc biệt thể hiện mối quan hệ "has-a" yếu (quan hệ một phần - Part of). Lớp thành phần (Part) có vòng đời độc lập và không bị hủy khi lớp chứa (Whole) bị hủy.
- **Ký pháp**: Đường nét liền với hình thoi rỗng ở đầu lớp chứa (Whole) và kết nối với lớp thành phần (Part).

### 5. Hợp thành (Composition)
- **Ý nghĩa**: Một dạng liên kết đặc biệt mạnh mẽ thể hiện mối quan hệ "has-a" mạnh (quan hệ bộ phận sống còn). Lớp thành phần (Part) không thể tồn tại độc lập và sẽ bị hủy cùng lúc khi lớp chứa (Whole) bị hủy.
- **Ký pháp**: Đường nét liền với hình thoi đặc ở đầu lớp chứa (Whole) và kết nối với lớp thành phần (Part).

### 6. Phụ thuộc (Dependency)
- **Ý nghĩa**: Thể hiện lớp này sử dụng đối tượng của lớp kia tạm thời (ví dụ: làm tham số truyền vào phương thức, hoặc biến cục bộ bên trong hàm) mà không lưu trữ dưới dạng thuộc tính (member field). Sự thay đổi định nghĩa ở lớp bị phụ thuộc có thể ảnh hưởng đến lớp sử dụng.
- **Ký pháp**: Đường nét đứt với đầu mũi tên nhọn rỗng (`..>`) trỏ từ lớp sử dụng sang lớp bị phụ thuộc.

---

## IV. BỘ TIÊU CHUẨN KIỂM TRA LỖI KIẾN TRÚC (ARCHITECTURAL CHECKLIST FOR AI)

AI bắt buộc phải rà soát cấu trúc JSON và phân tích sơ đồ lớp dựa trên 7 tiêu chuẩn kiểm định kiến trúc và hướng dẫn xử lý chi tiết sau:

1. **God Class (Lớp vạn năng)**:
   * **Dấu hiệu**: Một lớp chứa quá nhiều thuộc tính (> 10 thuộc tính) hoặc quá nhiều phương thức (> 10 phương thức), ôm đồm nhiều nghiệp vụ không liên quan.
   * **Cách AI kiểm tra**: Đọc số lượng thuộc tính và phương thức của từng lớp. Chỉ danh lớp quá tải, giải thích trách nhiệm mà lớp đó đang gánh vác sai.
   * **Giải pháp bắt buộc**: Đề xuất tách lớp lớn thành các lớp nhỏ hơn có trách nhiệm duy nhất (Single Responsibility Principle - SRP) và thiết lập liên kết giữa chúng.

2. **Anemic Domain Model (Mô hình thiếu máu)**:
   * **Dấu hiệu**: Lớp thực thể (Domain Entity/Model) chỉ chứa các thuộc tính dữ liệu và hàm get/set thuần túy mà hoàn toàn không có bất kỳ phương thức xử lý logic nghiệp vụ hay kiểm tra ràng buộc nào. Logic nghiệp vụ bị đẩy hết sang các lớp Service (Transaction Script).
   * **Cách AI kiểm tra**: Rà soát các lớp có tên hoặc vai trò là Entity/Model, kiểm tra xem mảng `methods` của chúng có trống hoặc chỉ có hàm getter/setter hay không.
   * **Giải pháp bắt buộc**: Đề xuất chuyển dịch các phương thức chứa logic nghiệp vụ và ràng buộc dữ liệu tương ứng từ tầng Service vào bên trong lớp Entity để đảm bảo tính đóng gói (Encapsulation).

3. **Phụ thuộc vòng (Cyclic Dependency)**:
   * **Dấu hiệu**: Xuất hiện chu trình khép kín giữa các liên kết của các lớp (ví dụ: Lớp A tham chiếu B, Lớp B tham chiếu ngược lại A; hoặc A -> B -> C -> A).
   * **Cách AI kiểm tra**: Dò theo các mối quan hệ `association`, `dependency` trong JSON để tìm vòng khép kín.
   * **Giải pháp bắt buộc**: Đề xuất sử dụng nguyên lý đảo ngược phụ thuộc (DIP) bằng cách tạo Interface trung gian hoặc gộp hai lớp nếu chúng có mối liên kết quá khăng khít.

4. **Shotgun Surgery (Phẫu thuật phát súng)**:
   * **Dấu hiệu**: Một thay đổi nhỏ về nghiệp vụ đòi hỏi phải chỉnh sửa rải rác nhiều lớp khác nhau do liên kết quá chặt chẽ (Tight Coupling) và độ gắn kết thấp (Low Cohesion).
   * **Cách AI kiểm tra**: Tìm các cụm lớp có sự phụ thuộc quá chằng chịt hoặc các lớp có thuộc tính riêng lẻ lặp lại nhưng nằm ở nhiều lớp khác nhau.
   * **Giải pháp bắt buộc**: Đề xuất gom các thuộc tính/hành vi liên quan mật thiết vào một lớp chung hoặc sử dụng một mẫu thiết kế (như Facade hoặc Mediator) để làm trung gian điều phối.

5. **Lạm dụng kế thừa (Inheritance over Composition)**:
   * **Dấu hiệu**: Một lớp sử dụng quan hệ kế thừa (`--|>`) với lớp khác chỉ nhằm mục đích tái sử dụng mã nguồn/thuộc tính có sẵn, trong khi quan hệ logic thực tế không thỏa mãn tính chất "is-a" (Con là một dạng cụ thể của Cha).
   * **Cách AI kiểm tra**: Phân tích quan hệ kế thừa giữa lớp con và lớp cha xem có thực sự mang tính phân loại hay chỉ là sao chép cấu trúc.
   * **Giải pháp bắt buộc**: Đề xuất chuyển đổi quan hệ kế thừa sang quan hệ chứa trong (Composition `*--` hoặc Aggregation `o--`) để giữ tính đóng gói linh hoạt.

6. **Vi phạm nguyên lý đảo ngược phụ thuộc (Dependency Inversion Principle - DIP)**:
   * **Dấu hiệu**: Lớp dịch vụ cấp cao (như Service, Controller) khai báo các thuộc tính phụ thuộc hoặc tham chiếu trực tiếp vào các lớp triển khai cụ thể ở tầng cấp thấp (như SQLRepository, GmailSender) thay vì phụ thuộc vào các Abstraction (Interface/Abstract Class).
   * **Cách AI kiểm tra**: Kiểm tra xem các thuộc tính/phương thức của lớp cấp cao có tham chiếu trực tiếp đến tên của concrete classes ở tầng dưới hay không.
   * **Giải pháp bắt buộc**: Đề xuất khai báo Interface ở giữa (ví dụ: `IRepository`, `IEmailSender`) và cho lớp cấp cao phụ thuộc vào Interface đó, để lớp cấp thấp tự triển khai cụ thể (DIP).

7. **Sai chiều mũi tên quan hệ**:
   * **Quy tắc bắt buộc**: 
     - Chiều mũi tên Thừa kế (`Con --|> Cha`) và Hiện thực hóa (`Lớp cụ thể ..|> Interface`) **bắt buộc phải trỏ về phía lớp Cha hoặc Interface**.
     - Chiều quan hệ Phụ thuộc (`A ..> B`) và Liên kết (`A --> B`) **bắt buộc phải trỏ từ lớp sử dụng (A) sang lớp bị sử dụng (B)**.
   * **Cách AI kiểm tra**: Đọc chiều mũi tên trong danh sách quan hệ JSON, tìm các mối quan hệ bị khai báo ngược hướng logic hoặc sai ký hiệu đầu mũi tên.
   * **Giải pháp bắt buộc**: Chỉ ra cụ thể cặp lớp có hướng mũi tên bị ngược và yêu cầu vẽ lại đúng chiều chuẩn UML.
   