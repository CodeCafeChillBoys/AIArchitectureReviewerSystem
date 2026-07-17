# Hướng dẫn Chi tiết về Các Mối quan hệ trong Sơ đồ (Diagram Relationships)

Tài liệu này cung cấp định nghĩa, ý nghĩa ngữ nghĩa, cú pháp Mermaid.js, chiều mũi tên và các lỗi thiết kế phổ biến cho các mối quan hệ trong Class Diagram, Sequence Diagram, ERD, và Use Case Diagram.

---

## I. SƠ ĐỒ LỚP (CLASS DIAGRAM)

Các mối quan hệ trong Class Diagram thể hiện cấu trúc liên kết và sự phụ thuộc tĩnh giữa các lớp.

| Tên mối quan hệ | Cú pháp Mermaid | Ý nghĩa ngữ nghĩa (Semantics) | Hướng mũi tên (Direction) |
|---|---|---|---|
| **Kế thừa (Inheritance / Generalization)** | `Con --|> Cha` | Thể hiện quan hệ "is-a" (Con là một loại Cha). Con thừa hưởng toàn bộ thuộc tính/phương thức của Cha. | **Từ Con trỏ về Cha**. Lớp con phụ thuộc vào lớp cha. |
| **Hiện thực hóa (Realization)** | `Class ..|> Interface` | Lớp cụ thể triển khai/hiện thực các phương thức khai báo trong Interface. | **Từ Lớp cụ thể trỏ về Interface**. |
| **Liên kết (Association)** | `A --> B` (1 chiều)<br>`A -- B` (2 chiều) | Một lớp giữ tham chiếu đến lớp kia dưới dạng thuộc tính (trường dữ liệu) để gọi phương thức. | **Từ lớp gửi yêu cầu (chứa thuộc tính) trỏ về lớp đích**. |
| **Phụ thuộc (Dependency)** | `A ..> B` | Lớp A sử dụng lớp B làm tham số truyền vào hàm hoặc biến cục bộ tạm thời. B thay đổi có thể làm ảnh hưởng đến A. | **Từ lớp sử dụng (A) trỏ về lớp bị sử dụng (B)**. |
| **Thu nạp (Aggregation)** | `Cha o-- Con` (Whole o-- Part) | Thể hiện quan hệ "has-a" dạng yếu. Con là một phần của Cha nhưng có thể tồn tại độc lập khi Cha bị hủy. | **Đầu hình thoi rỗng nằm ở lớp chứa (Whole)** trỏ về lớp thành phần (Part). |
| **Hợp thành (Composition)** | `Cha *-- Con` (Whole *-- Part) | Thể hiện quan hệ "has-a" dạng mạnh mẽ. Con là một phần sống còn của Cha và không thể tồn tại độc lập (cùng sinh cùng tử). | **Đầu hình thoi đặc nằm ở lớp chứa (Whole)** trỏ về lớp thành phần (Part). |

### Các lỗi thường gặp về mối quan hệ trong Class Diagram:
1. **Đảo ngược chiều kế thừa**: Vẽ mũi tên trỏ từ Cha sang Con (Sai). Mũi tên kế thừa luôn luôn phải trỏ về lớp Cha.
2. **Nhầm lẫn giữa Aggregation và Composition**: Dùng Aggregation khi vòng đời của thành phần con phụ thuộc hoàn toàn vào cha (ví dụ: `Order *-- OrderLine` là Composition chứ không phải Aggregation).
3. **Mũi tên liên kết vòng (Cyclic Dependency)**: Lớp A trỏ đến B qua quan hệ Association, và B lại trỏ ngược lại A qua Association. Cần tách Interface để gỡ bỏ phụ thuộc vòng.

---

## II. SƠ ĐỒ TUẦN TỰ (SEQUENCE DIAGRAM)

Mối quan hệ trong Sequence Diagram được thể hiện bằng dòng chảy của các thông điệp (Messages) giữa các Lifeline theo thời gian.

| Loại tin nhắn / Mối quan hệ | Cú pháp Mermaid | Ý nghĩa ngữ nghĩa (Semantics) | Hướng mũi tên (Direction) |
|---|---|---|---|
| **Tin nhắn đồng bộ (Sync Call)** | `A ->> B: Method()` | Đối tượng A gọi hàm của đối tượng B và bị chặn (đợi) cho đến khi B xử lý xong và phản hồi. | **Từ đối tượng gửi sang đối tượng nhận**. |
| **Tin nhắn bất đồng bộ (Async Call)** | `A -) B: Method()` | Đối tượng A gửi yêu cầu cho B và tiếp tục công việc của mình ngay lập tức mà không cần đợi. | **Từ đối tượng gửi sang đối tượng nhận**. |
| **Tin nhắn phản hồi (Return)** | `B -->> A: Response` | Trả kết quả từ đối tượng nhận (B) về đối tượng gửi ban đầu (A) sau khi xử lý xong. | **Từ đối tượng nhận trỏ ngược lại đối tượng gửi**. Vẽ bằng nét đứt. |
| **Tự tương tác (Self-Call)** | `A ->> A: LocalMethod()` | Đối tượng tự gọi phương thức nội bộ của chính mình. | **Mũi tên tự vòng lại chính nó**. |

### Các lỗi thường gặp trong Sequence Diagram:
1. **Sai chiều mũi tên phản hồi (Return)**: Vẽ mũi tên phản hồi hướng từ A sang B (Sai). Mũi tên nét đứt phản hồi phải chỉ ngược lại từ B về A.
2. **Hardcode tin nhắn CSDL trực tiếp**: Vẽ mũi tên gọi hàm trực tiếp từ Client/Controller xuống Database/Repository mà không qua Service Layer.

---

## III. SƠ ĐỒ QUAN HỆ THỰC THỂ (ERD / DATABASE DIAGRAM)

Mối quan hệ trong ERD thể hiện ràng buộc dữ liệu (khóa ngoại) và lực lượng (cardinalities) giữa các bảng.

| Loại quan hệ | Ký hiệu Mermaid | Ý nghĩa ngữ nghĩa (Semantics) | Nguyên tắc thiết kế khóa ngoại (FK) |
|---|---|---|---|
| **Một - Một (1:1)** | `TableA \|\|--\|\| TableB` | Một dòng ở bảng A chỉ liên kết với duy nhất một dòng ở bảng B. | Khóa ngoại có thể nằm ở bảng A hoặc bảng B, kèm ràng buộc UNIQUE. |
| **Một - Nhiều (1:N)** | `Parent \|\|--\|{ Child` | Một dòng ở bảng Cha có thể liên kết với nhiều dòng ở bảng Con. | **Khóa ngoại bắt buộc phải nằm ở bảng Con** (đầu Nhiều) trỏ về bảng Cha. |
| **Nhiều - Nhiều (N:M)** | `TableA }\|--\|{ TableB` | Nhiều dòng ở bảng A liên kết với nhiều dòng ở bảng B. | **Tuyệt đối không thiết kế trực tiếp**. Phải tách thành hai quan hệ 1-N thông qua một bảng trung gian (Junction Table). |

### Các lỗi thường gặp trong ERD:
1. **Đặt sai đầu khóa ngoại**: Đặt khóa ngoại ở bảng 1 thay vì bảng Nhiều (ví dụ: đặt `OrderId` trong bảng `Customer` thay vì đặt `CustomerId` trong bảng `Order`).
2. **Không giải quyết quan hệ Nhiều-Nhiều**: Vẽ quan hệ Nhiều-Nhiều trực tiếp giữa hai bảng nghiệp vụ lớn mà không tạo bảng liên kết ở giữa.

---

## IV. SƠ ĐỒ CA SỬ DỤNG (USE CASE DIAGRAM)

Mối quan hệ trong Use Case Diagram thể hiện sự phân rã chức năng và quyền hạn tương tác của tác nhân.

| Tên mối quan hệ | Cú pháp Mermaid | Ý nghĩa ngữ nghĩa (Semantics) | Hướng mũi tên (Direction) |
|---|---|---|---|
| **Tương tác (Association)** | `Actor --- UseCase` | Thể hiện tác nhân có quyền thực hiện hoặc tương tác với ca sử dụng. | **Đường nét liền, không có đầu mũi tên**. |
| **Bao hàm (<<include>>)** | `BaseUseCase --> IncludedUseCase` | Use Case gốc bắt buộc phải gọi Use Case bao hàm để có thể hoàn thành tác vụ. | **Từ Use Case gốc trỏ đến Use Case được bao hàm**. |
| **Mở rộng (<<extend>>)** | `ExtendingUseCase --> BaseUseCase` | Use Case mở rộng bổ sung chức năng cho Use Case gốc khi thỏa mãn một điều kiện cụ thể. | **BẮT BUỘC từ Use Case mở rộng trỏ về Use Case gốc**. |
| **Kế thừa (Generalization)** | `ActorCon --\|> ActorCha`<br>`UCCon --\|> UCCha` | Thể hiện sự chuyên biệt hóa của Actor hoặc Use Case. | **Từ Con trỏ về Cha**. |

### Các lỗi thường gặp trong Use Case Diagram:
1. **Vẽ mũi tên giữa Actor và Use Case**: Vẽ đường liên kết có đầu mũi tên chỉ từ Actor vào Use Case (Sai). Đường này phải là nét liền không mũi tên.
2. **Đảo ngược chiều mũi tên `<<extend>>`**: Vẽ mũi tên `<<extend>>` chỉ từ Use Case gốc sang Use Case mở rộng (Sai). Mũi tên phải chỉ ngược lại từ Use Case mở rộng về Use Case gốc.
3. **Dùng sai bản chất `<<include>>`**: Sử dụng `<<include>>` để mô tả trình tự thực hiện các bước tuần tự (ví dụ: `Đăng nhập` include `Xem trang chủ`).