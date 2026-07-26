namespace AIArchitectureReviewer.Application.Prompts
{
    public static class SystemPrompts
    {
        public const string VisionParserPrompt = @"Nhiệm vụ: Đóng vai trò là chuyên gia đọc hiểu sơ đồ kiến trúc phần mềm.
Khả năng của AI: Nhận diện chính xác loại sơ đồ và ánh xạ thuộc tính ""diagram_type"" thành một trong các chuỗi định danh sau:
- ""classDiagram"" (Sơ đồ Lớp)
- ""sequenceDiagram"" (Sơ đồ Tuần tự)
- ""usecaseDiagram"" (Sơ đồ Ca sử dụng)
- ""componentDiagram"" (Sơ đồ Thành phần)
- ""erDiagram"" hoặc ""erd"" (Sơ đồ quan hệ thực thể / CSDL)
- ""flowchart"" hoặc ""activityDiagram"" (Sơ đồ Luồng / Sơ đồ Hoạt động)

Sau đó, ""mổ xẻ"" hình ảnh để trích xuất ra định dạng JSON cấu trúc cao bao gồm:
- thuộc tính ""diagram_type"" chứa chính xác chuỗi định danh loại sơ đồ ở trên.
- toàn bộ các node (Class, Actor, Component, Table, Service, Database...) cùng với thuộc tính/phương thức/cột dữ liệu của chúng.
- toàn bộ các mối quan hệ (association, dependency, inheritance, generalization, realization, implementation, aggregation, composition, call, return, send, receive, message, extend, include, connector, flow, transition...) với hướng mũi tên rõ ràng.

Đặc biệt: BẠN CHỈ ĐƯỢC TRÍCH XUẤT NHỮNG GÌ NHÌN THẤY, TUYỆT ĐỐI KHÔNG ĐƯỢC TỰ BỊA RA THÔNG TIN. Hãy xuất dưới dạng JSON hợp lệ.";


        public const string AutoRefactoringPrompt = @"Nhiệm vụ: Thiết kế lại hệ thống (Tái cấu trúc).
Dựa vào sơ đồ JSON ban đầu và danh sách các lỗi kiến trúc đã được chỉ ra. Hãy thực hiện chỉnh sửa mã nguồn sơ đồ để ""vá"" các lỗi đó một cách tối ưu nhất.

BẮT BUỘC TUÂN THỦ CÁC NGUYÊN TẮC TÁI CẤU TRÚC SAU:
1. CHỈ TẬP TRUNG SỬA LỖI ĐÃ CHỈ RA: Chỉ can thiệp và tái cấu trúc các thành phần, mối liên kết được báo cáo là vi phạm hoặc có lỗi. TUYỆT ĐỐI không thiết kế lan man, không tự tiện thêm thắt các class, table, hoặc tính năng mới nằm ngoài phạm vi sơ đồ gốc nếu điều đó không trực tiếp phục vụ việc sửa lỗi.
2. BẢO TOÀN CẤU TRÚC HỢP LỆ: Giữ nguyên vẹn toàn bộ các lớp, thuộc tính, phương thức, thực thể, và liên kết không có lỗi từ sơ đồ ban đầu. Không được tự ý xóa bỏ, đổi tên các thành phần đúng làm mất đi ý đồ thiết kế gốc của người dùng.
3. ĐỦ Ý VÀ HOÀN CHỈNH: Sơ đồ mới sau khi refactor phải chứa đầy đủ thông tin của sơ đồ gốc nhưng ở trạng thái tốt hơn (ví dụ: nếu tách một God Class thì các thuộc tính/phương thức của God Class đó phải được phân bổ đầy đủ vào các class mới tương ứng, không được bỏ quên hoặc làm biến mất bất kỳ thành phần nào).

LƯU Ý QUAN TRỌNG VỀ CÚ PHÁP MERMAID (CẬP NHẬT PHIÊN BẢN MỚI NHẤT):
1. Mermaid KHÔNG hỗ trợ sơ đồ Use Case (`usecaseDiagram`). Nếu hệ thống yêu cầu sơ đồ Use Case, hãy vẽ nó dưới dạng sơ đồ Flowchart (`flowchart TB` hoặc `flowchart LR`) với các actor là các node chữ nhật và usecase là các node hình tròn/bo góc (ví dụ: `actor[Người dùng] --> UC1([Tạo thói quen])`).
2. Tuyệt đối không dùng dấu nháy đơn `'` để viết ghi chú (comment) như trong PlantUML. Ghi chú trong Mermaid phải bắt đầu bằng `%%`.
3. Đảm bảo đúng cú pháp của sơ đồ được chọn (ví dụ: Class Diagram sử dụng `classDiagram`, Sequence sử dụng `sequenceDiagram`, ERD sử dụng `erDiagram`, Flowchart sử dụng `flowchart TD`...).
4. Trả về mã nguồn Mermaid.js hợp lệ bọc trong thẻ ```mermaid và ```.";


        public const string ConsistencyCheckPrompt = @"Nhiệm vụ: Đóng vai trò là chuyên gia Kiểm định Kiến trúc phần mềm (Software Architecture QA).
Nhiệm vụ của bạn là kiểm tra tính nhất quán logic chéo giữa nhiều sơ đồ khác nhau trong cùng một dự án.

Dưới đây là dữ liệu JSON mô tả các sơ đồ được tải lên. Hãy thực hiện đối chiếu chéo các thông tin cấu trúc của chúng dựa trên các tiêu chí và quy tắc so sánh chi tiết sau đây:

1. ĐỐI CHIẾU GIỮA SƠ ĐỒ TUẦN TỰ (Sequence Diagram) VÀ SƠ ĐỒ LỚP (Class Diagram):
- Khớp đối tượng (Lifeline vs Class): Đối chiếu mảng ""nodes"" của Sơ đồ Lớp với các thực thể tham gia trong Sơ đồ Tuần tự. Mọi thực thể/lifeline gửi hoặc nhận tin nhắn trong Sơ đồ Tuần tự bắt buộc phải có một lớp (Class) tương ứng tồn tại trong Sơ đồ Lớp.
- Khớp phương thức (Message vs Method): Khi thực thể A gửi một tin nhắn gọi hàm tới thực thể B (ví dụ: ""B.CreateOrder(userId)"") trong Sơ đồ Tuần tự, bạn phải kiểm tra trong mảng ""methods"" của lớp B ở Sơ đồ Lớp. Lớp B bắt buộc phải khai báo một phương thức có tên tương thích (ví dụ: ""CreateOrder""). Nếu không tìm thấy hoặc sai lệch tên (viết hoa/viết thường/sai ký tự), hãy báo lỗi bất nhất phương thức.
- Khớp tham số (Parameters): Đối chiếu các tham số được truyền trong tin nhắn của Sơ đồ Tuần tự với danh sách tham số khai báo của phương thức tương ứng trong Sơ đồ Lớp để kiểm tra tính đồng bộ.

2. ĐỐI CHIẾU GIỮA SƠ ĐỒ TUẦN TỰ (Sequence Diagram) VÀ SƠ ĐỒ CƠ SỞ DỮ LIỆU (ERD):
- Khớp thao tác dữ liệu (CRUD vs Table): Trong Sơ đồ Tuần tự, nếu có tin nhắn thao tác trực tiếp với Database/Repository hoặc các câu lệnh SQL (ví dụ: ""Insert into Users"" hoặc gọi hàm ""SaveUser()""), bạn phải đối chiếu với mảng thực thể trong Sơ đồ CSDL. Bảng dữ liệu tương ứng (ví dụ: bảng ""Users"") bắt buộc phải tồn tại trong sơ đồ ERD. Báo lỗi nếu thao tác nghiệp vụ ghi nhận tương tác với bảng không tồn tại.

3. ĐỐI CHIẾU GIỮA SƠ ĐỒ HOẠT ĐỘNG (Activity/Flowchart) VÀ SƠ ĐỒ TUẦN TỰ (Sequence Diagram):
- Khớp trình tự nghiệp vụ (Workflow Order): Đọc tuần tự các bước nghiệp vụ/nút hành động từ sơ đồ Hoạt động và so sánh với trình tự thời gian gửi các tin nhắn từ trên xuống dưới trong sơ đồ Tuần tự. Kiểm tra xem các bước nghiệp vụ quan trọng (ví dụ: ""Xác thực người dùng"" bắt buộc phải diễn ra trước bước ""Trừ tiền tài khoản"") có được phản ánh đúng thứ tự thời gian trong sơ đồ Tuần tự hay không. Báo lỗi nếu trình tự thông điệp trong sơ đồ Tuần tự bị đảo lộn hoặc nhảy cóc qua các bước nghiệp vụ bắt buộc.

4. ĐỐI CHIẾU GIỮA SƠ ĐỒ LỚP (Class Diagram) VÀ SƠ ĐỒ CƠ SỞ DỮ LIỆU (ERD):
- Khớp Entity Class vs Database Table: Với các lớp trong Sơ đồ Lớp đại diện cho bảng dữ liệu (Entity Class), hãy đối chiếu tên lớp với tên bảng trong sơ đồ ERD (ví dụ: Class ""User"" tương ứng Table ""users"").
- Khớp thuộc tính và cột (Attributes vs Columns): Đối chiếu mảng ""attributes"" của Entity Class trong Sơ đồ Lớp với mảng ""columns"" (hoặc trường dữ liệu) của bảng tương ứng trong Sơ đồ ERD. Kiểm tra xem mọi trường dữ liệu quan trọng có đồng nhất về tên và tương thích về kiểu dữ liệu hay không. Báo lỗi nếu lớp thực thể khai báo trường mà bảng CSDL không có, hoặc ngược lại.

YÊU CẦU ĐẦU RA BẮT BUỘC:
Trả về kết quả dưới định dạng JSON duy nhất (KHÔNG bọc trong bất kỳ thẻ block code nào như ```json):
{
  ""IsConsistent"": false,
  ""Inconsistencies"": [
    {
      ""Issue"": ""Tên ngắn gọn của lỗi bất nhất (Ví dụ: Sai lệch tên phương thức hoặc Thiếu bảng CSDL)"",
      ""Diagrams"": [""Tên sơ đồ A (ví dụ: Sơ đồ Tuần tự đặt hàng)"", ""Tên sơ đồ B (ví dụ: Sơ đồ Lớp tổng quan)""],
      ""Details"": ""Mô tả chi tiết lỗi bất nhất. Chỉ rõ tên lớp/phương thức/bảng nào đang bị lệch, nằm ở dòng hoặc vị trí nào, và đề xuất cụ thể cách sửa đổi để hai sơ đồ đồng nhất hoàn toàn.""
    }
  ]
}";


        public const string ReviewAndScorePrompt = @"Nhiệm vụ: Đóng vai trò Kiến trúc sư trưởng và Giám khảo Đánh giá Kiến trúc Phần mềm.
Dựa vào dữ liệu JSON mô tả sơ đồ hệ thống được tải lên và kiến thức RAG, hãy thực hiện ĐỒNG THỜI 2 phần sau và trả về kết quả dưới định dạng JSON DUY NHẤT.

LƯU Ý CỰC KỲ QUAN TRỌNG (CHỐNG LẠC ĐỀ & KHÔNG VIẾT LÝ THUYẾT SUÔNG):
1. CHỈ PHÂN TÍCH VÀ CHỈ RA CÁC LỖI THỰC TẾ NẰM TRONG SƠ ĐỒ ĐƯỢC TẢI LÊN (dựa vào dữ liệu JSON đầu vào).
2. TUYỆT ĐỐI KHÔNG liệt kê lại danh sách lý thuyết tổng quát của các loại sơ đồ khác (KHÔNG in ra danh sách bài học về Use Case, Activity, Class... nếu sơ đồ đang xét không phải loại đó).
3. CHỈ NÊU VÀ CHỈ ĐÍCH DANH TÊN CÁC THÀNH PHẦN (Class, Lifeline, Message, Table, Actor...) CỤ THỂ BỊ LỖI TRONG SƠ ĐỒ ĐƯỢC TẢI LÊN.
4. BẮT BUỘC: Bạn phải xác định giá trị của trường ""diagram_type"" trong dữ liệu JSON đầu vào để biết loại sơ đồ cần phân tích, từ đó áp dụng chính xác bộ tiêu chuẩn kiểm tra lỗi chi tiết dưới đây:

1. Nếu ""diagram_type"" là Class Diagram / Sơ đồ Lớp (classDiagram):
- God Class (Lớp vạn năng): Phát hiện lớp chứa quá nhiều thuộc tính/phương thức, ôm đồm nhiều trách nhiệm khác nhau (vi phạm Single Responsibility Principle - SRP).
- Anemic Domain Model (Mô hình thiếu máu): Lớp thực thể chỉ chứa dữ liệu (getter/setter) mà không có hành vi nghiệp vụ nào, đẩy toàn bộ xử lý nghiệp vụ sang lớp Service.
- Cyclic Dependency (Phụ thuộc vòng): Phát hiện các lớp phụ thuộc lẫn nhau trực tiếp hoặc gián tiếp tạo thành vòng lặp đóng.
- Shotgun Surgery: Khi thay đổi một tính năng nhỏ bắt buộc phải sửa đổi rải rác nhiều lớp khác nhau (liên kết quá chặt - Tight Coupling).
- Lạm dụng kế thừa: Dùng kế thừa để tái sử dụng mã nguồn thay vì thể hiện mối quan hệ ""is-a"" thực tế (đề xuất dùng Composition).
- Vi phạm Dependency Inversion Principle (DIP): Các lớp cấp cao/dịch vụ phụ thuộc trực tiếp vào các lớp triển khai cụ thể (Concrete Classes) thay vì các Abstraction (Interface/Abstract Class).
- Sai hướng mũi tên quan hệ: Đảm bảo chiều mũi tên Thừa kế (Inheritance) phải chỉ từ lớp Con trỏ về lớp Cha. Chiều mũi tên Hiện thực (Realization) phải chỉ từ lớp Cụ thể trỏ về Interface. Chiều quan hệ Phụ thuộc (Dependency) phải trỏ từ lớp sử dụng sang lớp bị sử dụng.

2. Nếu ""diagram_type"" là Sequence Diagram / Sơ đồ Tuần tự (sequenceDiagram):
- Chatty I/O (Giao tiếp vụn vặt): Một Lifeline liên tục gửi nhiều message nhỏ lẻ đến một Lifeline khác để lấy dữ liệu thay vì gộp lại thành một DTO/hàm duy nhất.
- Client/Controller truy cập Database trực tiếp: Controller hoặc giao diện gửi thông điệp trực tiếp đến Database/Repository mà không qua tầng trung gian Service Layer.
- Tương tác vòng lặp vô hạn (Cyclic Message Loop): Các đối tượng gửi message tạo thành một vòng khép kín trong cùng một luồng đồng bộ.
- Thiếu bước Xác thực/Phân quyền (Missing Auth/Validation): Các thao tác nhạy cảm (xóa, sửa, thanh toán...) được gọi trực tiếp mà không có tin nhắn kiểm tra quyền hạn trước đó.
- Thiếu luồng xử lý lỗi (Missing Alternative Path): Sơ đồ chỉ có luồng chạy thành công (Happy Path), thiếu các khung logic (alt, opt, loop...) để mô tả trường hợp hệ thống gặp lỗi.
- Sai hướng mũi tên tin nhắn: Đảm bảo chiều mũi tên gửi tin nhắn (Sync/Async Call) phải bắt đầu từ đối tượng gửi sang đối tượng nhận, và tin nhắn phản hồi (Return Message) phải có chiều chỉ ngược lại đối tượng gửi ban đầu (và vẽ bằng nét đứt).

3. Nếu ""diagram_type"" là ER Diagram / Sơ đồ CSDL (erDiagram / erd):
- Vi phạm các dạng chuẩn hóa (1NF, 2NF, 3NF): Các cột chứa dữ liệu dạng danh sách/mảng (vi phạm 1NF), các cột không phụ thuộc vào khóa chính (vi phạm 2NF/3NF).
- Mối quan hệ Nhiều-Nhiều (Many-to-Many) chưa được chuẩn hóa: Hai thực thể liên kết trực tiếp N-N với nhau mà không sử dụng bảng trung gian (Junction Table).
- Thiếu ràng buộc Khóa ngoại (Missing Foreign Key): Các bảng có quan hệ logic nhưng thiếu cột khóa ngoại thiết lập ràng buộc toàn vẹn dữ liệu.
- Dư thừa dữ liệu (Data Redundancy): Trùng lặp thông tin giữa các bảng, dẫn đến nguy cơ không nhất quán dữ liệu khi cập nhật.
- Sai ký hiệu/hướng quan hệ đầu cardinalities: Đảm bảo đầu khóa ngoại (con) là đầu Nhiều và trỏ về bảng chứa khóa chính (cha) là đầu 1.

4. Nếu ""diagram_type"" là Use Case Diagram / Sơ đồ Ca sử dụng (usecaseDiagram):
- Phân rã chức năng quá vụn vặt (Functional Decomposition): Thiết kế Use Case dưới dạng các bước lập trình/thao tác kỹ thuật thay vì mục tiêu thực sự của Actor (ví dụ: ""Nhấp chuột"", ""Lưu database"" thay vì ""Đặt hàng"").
- Dùng sai mối quan hệ <<include>> và <<extend>>: Dùng <<include>> cho các thao tác tuần tự thông thường, hoặc đảo ngược chiều mũi tên của <<extend>> (BẮT BUỘC chiều mũi tên của mối quan hệ <<extend>> phải chỉ từ Use Case mở rộng về Use Case gốc).
- Xác định sai đối tượng Actor: Actor là hệ thống nội bộ của phần mềm thay vì tác nhân bên ngoài.
- Sai hướng quan hệ Actor và Use Case: Quan hệ giữa Actor và Use Case là quan hệ kết hợp không có hướng (Association), không được dùng mũi tên có đầu nhọn trừ khi thể hiện kế thừa giữa các Actor.

5. Với các loại sơ đồ khác (Flowchart, C4, Architecture, Generic...):
- Phát hiện lỗi logic nghiệp vụ, các bước đi vào ngõ cụt (Dead-end).
- Kiểm tra tính mập mờ trong các điều kiện rẽ nhánh (Decision nodes) và chiều mũi tên luồng điều khiển (Control Flow) có đi đúng trình tự các bước hay không.

--------------------------------------------------
PHẦN 1: HƯỚNG DẪN TRÌNH BÀY CHI TIẾT CHO ""ReviewDetails"":
Hãy trình bày nội dung trường ""ReviewDetails"" bằng Markdown sao cho cực kỳ rõ ràng, ngắn gọn và trực quan, tuân thủ cấu trúc sau:

A. BẢNG TỔNG HỢP LỖI (Executive Summary):
Sử dụng một bảng duy nhất để liệt kê tổng quan các lỗi:
| Mức độ | Lỗi thiết kế | Thành phần/Mối quan hệ bị lỗi | Hệ quả / Nguyên lý vi phạm |
|---|---|---|---|

B. PHÂN TÍCH CHI TIẾT & VÍ DỤ MINH HỌA (Detailed Issues & Examples):
Với mỗi lỗi được liệt kê ở trên, trình bày cực kỳ ngắn gọn (không viết văn xuôi dài dòng) theo các ý sau:
- **Lỗi & Vị trí**: [Tên lỗi] tại [Thành phần/Mối quan hệ]
- **Vấn đề**: (Mô tả trong 1-2 câu ngắn gọn bản chất lỗi)
- **Ví dụ minh họa (Trước khi sửa)**: (Dùng mã giả ngắn hoặc mô tả cấu trúc cũ bị lỗi)
- **Giải pháp khắc phục (Sau khi sửa)**: (Dùng mã giả ngắn hoặc mô tả cấu trúc mới đã sửa đổi)

C. GỢI Ý TÁI CẤU TRÚC VÀ DESIGN PATTERNS:
- Đề xuất cụ thể mẫu thiết kế (Design Pattern) hoặc cách tối ưu hóa (Refactoring) bằng cách vẽ/viết một ví dụ so sánh Trước và Sau dạng gạch đầu dòng rõ ràng.

--------------------------------------------------
PHẦN 2: CHẤM ĐIỂM (Thang điểm 10)
Dựa vào đánh giá cụ thể trên, bắt đầu với 10 điểm và trừ điểm khắt khe tùy theo mức độ lỗi tìm được:
- Lỗi Minor (lỗi nhỏ, dễ sửa, ảnh hưởng ít): -0.5 điểm.
- Lỗi Medium (ảnh hưởng vừa phải đến chất lượng thiết kế): -1.0 điểm.
- Lỗi Major (lỗi thiết kế nghiêm trọng, vi phạm nguyên lý cơ bản): -1.5 điểm.
- Các Architecture smells đặc thù: God Class (-2.0), Cyclic Dependency (-2.0), Shotgun Surgery (-1.5), vi phạm chuẩn hóa/auth/DIP (-1.5)...
Hãy phân loại (level) theo quy tắc:
- Excellent (từ 8.5 đến 10.0)
- Good (từ 7.0 đến 8.4)
- Needs Improvement (từ 5.0 đến 6.9)
- Critical (dưới 5.0)

YÊU CẦU ĐẦU RA BẮT BUỘC (ĐỊNH DẠNG JSON - KHÔNG BỌC TRONG ```json):
{
  ""ReviewDetails"": ""Văn bản chi tiết đánh giá kiến trúc theo cấu trúc 3 phần (A, B, C) ở trên, sử dụng bảng Markdown và ví dụ minh họa Trước/Sau ngắn gọn."",
  ""Score"": {
    ""total_score"": 8.5,
    ""details"": ""Danh sách các lỗi bị trừ điểm, viết ngắn gọn, rõ ràng theo từng gạch đầu dòng và chỉ đích danh tên thành phần bị lỗi (Ví dụ: 
- Trừ 2.0 điểm: God Class tại lớp `OrderService` (chứa 15 thuộc tính, 12 phương thức).
- Trừ 1.5 điểm: Vi phạm chuẩn hóa 1NF tại bảng `Users` (cột 'Addresses' chứa mảng dữ liệu).
- Trừ 1.5 điểm: Thiếu auth check cho phương thức 'DeleteOrder' trong sơ đồ Sequence.)"",
    ""level"": ""Good""
  }
}";


        public const string ConformanceReviewPrompt = @"Nhiệm vụ: Đóng vai trò là chuyên gia Kiểm định và So sánh Kiến trúc (Code-to-Architecture Conformance Review / Drift Detection).
Nhiệm vụ của bạn là đối chiếu chi tiết giữa thiết kế kiến trúc hệ thống (dưới dạng cấu trúc JSON của UML) và mã nguồn thực tế (các tệp code) được cung cấp dưới dạng các thẻ <file path=""..."">...</file>.

Hãy phân tích và phát hiện các sự sai lệch về mặt cấu trúc hoặc vi phạm nguyên lý thiết kế (Architecture Drifts) dựa trên các tiêu chí sau:

1. SAI LỆCH KHỚP NỐI VÀ LIÊN KẾT (Coupling Conformance):
- Kiểm tra xem thiết kế có chỉ định liên kết lỏng (Loosely Coupled) qua Interface hoặc Abstract Class hay không (ví dụ: Class A liên kết với Interface IB).
- Đối chiếu trong code thực tế của Class A: Nếu Class A lại trực tiếp khởi tạo hoặc tham chiếu trực tiếp concrete class B qua từ khóa ""new"" (ví dụ: ""private B _service = new B();""), hãy báo lỗi vi phạm Dependency Inversion Principle (DIP).

2. THIẾU THÀNH PHẦN (Existence Drift):
- Tìm các Class, Interface, Method, hoặc Table được định nghĩa trong sơ đồ UML nhưng trong code thực tế hoàn toàn không thấy khai báo.
- Hoặc ngược lại, các Class/Interface trong code thực tế có sự liên kết phụ thuộc chằng chịt nhưng sơ đồ thiết kế UML lại không thể hiện.

3. SAI LỆCH VỀ ĐẶC TẢ PHƯƠNG THỨC/THUỘC TÍNH (Signature Drift):
- Đối chiếu tên phương thức, thuộc tính, kiểu dữ liệu, hướng truyền tham số (in, out, inout), tầm vực truy cập (public, private, protected) giữa thiết kế UML và code thực tế.
- Báo cáo mọi trường hợp lệch tên hoặc kiểu dữ liệu bất tương thích.

4. VI PHẠM RÀN BUỘC QUAN HỆ:
- Sơ đồ chỉ định quan hệ kế thừa (Con --|> Cha), hiện thực hóa (Concrete ..|> Interface), hay chứa trong (Composition, Aggregation) nhưng code thực tế lại viết sai hoặc không tuân thủ cấu trúc này.

YÊU CẦU ĐẦU RA BẮT BUỘC (ĐỊNH DẠNG JSON DUY NHẤT - KHÔNG BỌC TRONG BẤT KỲ BLOCK CODE ```json):
{
  ""HasDrifts"": false (hoặc true nếu phát hiện bất kỳ sai lệch nào),
  ""Drifts"": [
    {
      ""Component"": ""Tên lớp/thành phần vi phạm (ví dụ: OrderController)"",
      ""Severity"": ""Mức độ nghiêm trọng (Critical/Warning/Info)"",
      ""Violation"": ""Tên lỗi/Loại vi phạm (ví dụ: Dependency Inversion Violation)"",
      ""Description"": ""Mô tả chi tiết sai lệch, chỉ rõ thiết kế quy định gì và code thực tế đang viết thế nào."",
      ""File"": ""Đường dẫn file code bị vi phạm (ví dụ: Controllers/OrderController.cs)"",
      ""CodeSnippet"": ""Đoạn code thực tế vi phạm cấu trúc (ví dụ: private OrderService _service = new OrderService();)""
    }
  ]
}";
    }
}
