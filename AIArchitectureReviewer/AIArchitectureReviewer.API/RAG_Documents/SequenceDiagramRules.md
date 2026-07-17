# Quy tắc Đánh giá và Hướng dẫn Chi tiết Sơ đồ Tuần tự (Sequence Diagram)

Tài liệu này cung cấp định nghĩa chuẩn hóa, các khái niệm tương tác động, ký pháp thông điệp và quy tắc đánh giá chi tiết cho Sơ đồ Tuần tự (Sequence Diagram) dựa trên tiêu chuẩn UML.

---

## I. THÀNH PHẦN CHÍNH CỦA SƠ ĐỒ (SEQUENCE DIAGRAM NOTATIONS)

Sơ đồ Tuần tự minh họa các tương tác động giữa các đối tượng trong hệ thống theo trình tự thời gian (chronological order), giúp chuyển đổi từ ca sử dụng cấp cao sang đặc tả mã nguồn. Sơ đồ gồm các thành phần:

1. **Đường đời (Lifeline)**:
   - Đại diện cho một đối tượng tham gia cụ thể vào kịch bản tương tác.
   - Ký pháp: Hình chữ nhật ghi tên đối tượng ở trên cùng với một đường kẻ nét đứt thẳng đứng đi xuống.

2. **Tác nhân (Actor)**:
   - Đại diện cho vai trò của thực thể bên ngoài kích hoạt kịch bản (người dùng, phần cứng ngoài, hoặc hệ thống bên ngoài).
   - Ký pháp: Hình người que đặt ở đầu đường đời tương ứng.

3. **Thanh kích hoạt (Activation Bar)**:
   - Biểu diễn khoảng thời gian một đối tượng đang thực sự thực thi một tác vụ nghiệp vụ hoặc phương thức.
   - Ký pháp: Hình chữ nhật mỏng màu trắng đè lên đường đời nét đứt.

---

## II. ĐẶC TẢ CÁC LOẠI THÔNG ĐIỆP (MESSAGE TYPES & DIRECTIONS)

Dòng chảy tương tác được biểu diễn thông qua các thông điệp (Messages) truyền tải giữa các Lifeline:

1. **Thông điệp gọi hàm đồng bộ (Call Message)**:
   - Gọi một phương thức trên đối tượng đích. Caller sẽ đợi (bị chặn) cho đến khi nhận được phản hồi.
   - **Ký pháp**: Đường nét liền với đầu mũi tên nhọn đặc hoặc mũi tên rỗng tam giác trỏ từ đối tượng gửi sang nhận.

2. **Thông điệp phản hồi (Return Message)**:
   - Trả dữ liệu hoặc quyền điều khiển về cho đối tượng gọi sau khi xử lý xong.
   - **Ký pháp**: Đường nét đứt với đầu mũi tên nhọn trỏ ngược lại từ đối tượng nhận về đối tượng gọi.

3. **Thông điệp bất đồng bộ (Asynchronous Message)**:
   - Caller gửi yêu cầu đi và tiếp tục xử lý công việc riêng mà không cần đợi đối tượng đích phản hồi.
   - **Ký pháp**: Đường nét liền với đầu mũi tên nhọn một nửa nét.

4. **Thông điệp tự gọi (Self Message)**:
   - Gọi phương thức nội bộ trong chính đối tượng đó.
   - **Ký pháp**: Đường mũi tên vòng ngược lại chính Lifeline đó.

5. **Thông điệp đệ quy (Recursive Message)**:
   - Một dạng tự gọi đặc biệt, kích hoạt một Thanh kích hoạt (Activation Bar) mới nằm đè lên thanh kích hoạt hiện tại.

6. **Thông điệp khởi tạo (Create Message)**:
   - Tạo mới/khởi tạo một đối tượng (Lifeline) mới trong luồng xử lý.
   - **Ký pháp**: Đường nét đứt trỏ thẳng vào hộp tên của Lifeline mới được tạo.

7. **Thông điệp hủy (Destroy Message)**:
   - Yêu cầu giải phóng/hủy vòng đời của đối tượng đích.
   - **Ký pháp**: Đường trỏ đến đường đời đích và kết thúc bằng ký hiệu chữ X lớn.

---

## III. KHUNG TƯƠNG TÁC (INTERACTION FRAGMENTS)

Sử dụng các khung logic để đặc tả cấu trúc điều khiển:
- **`alt` (Alternatives)**: Rẽ nhánh điều kiện (giống `if-else`). Chỉ một nhánh được thực thi.
- **`opt` (Options)**: Luồng xử lý tùy chọn, chỉ thực thi nếu điều kiện đúng (giống `if` đơn lẻ).
- **`loop`**: Khung lặp lại thông điệp một số lần hoặc theo điều kiện.

---

## IV. BỘ TIÊU CHUẨN KIỂM TRA LỖI KIẾN TRÚC CHO AI (CHECKLIST)

AI bắt buộc phải rà soát cấu trúc JSON và phân tích sơ đồ Tuần tự dựa trên các tiêu chuẩn kiểm định sau:

1. **Chatty I/O (Giao tiếp vụn vặt)**:
   * **Dấu hiệu**: Một đối tượng gửi quá nhiều cuộc gọi nhỏ liên tiếp tới đối tượng khác chỉ để lấy dữ liệu nhỏ lẻ (ví dụ: `GetName()`, `GetAge()`, `GetAddress()`).
   * **Cách kiểm tra**: Đếm số lượng message liên tục giữa 2 Lifeline cụ thể. Nếu có từ 3 tin nhắn nhỏ trở lên, báo lỗi.
   * **Giải pháp**: Yêu cầu thiết kế lại bằng cách gộp các phương thức thành một cuộc gọi lớn duy nhất hoặc sử dụng DDTO (Data Transfer Object).

2. **Client/Controller truy cập Database trực tiếp**:
   * **Dấu hiệu**: Controller hoặc giao diện UI gửi thông điệp trực tiếp đến Database/Repository hoặc gọi trực tiếp truy vấn SQL mà không qua lớp dịch vụ trung gian (Service Layer).
   * **Cách kiểm tra**: Phát hiện các message nối từ Controller/UI Lifeline trực tiếp đến Database/Repository Lifeline.
   * **Giải pháp**: Yêu cầu thêm Service Layer ở giữa để điều phối logic nghiệp vụ và phân tách tầng.

3. **Giao tiếp vòng lặp vô hạn (Cyclic Message Loop)**:
   * **Dấu hiệu**: Các thông điệp đồng bộ gọi lẫn nhau tạo thành chu trình khép kín trong cùng một luồng (ví dụ: A gọi B, B gọi C, C gọi lại A).
   * **Cách kiểm tra**: Duyệt danh sách các tin nhắn đồng bộ từ trên xuống dưới, tìm vòng lặp các đối tượng gọi nhau.
   * **Giải pháp**: Yêu cầu loại bỏ vòng lặp bằng cách chuyển sang giao tiếp bất đồng bộ (sử dụng Event/Message Queue) hoặc cấu trúc lại phân vùng trách nhiệm của các đối tượng.

4. **Thiếu bước Xác thực/Phân quyền (Missing Auth/Validation)**:
   * **Dấu hiệu**: Các tác vụ nhạy cảm, làm thay đổi dữ liệu lớn (như xóa đơn hàng, cập nhật số dư, trừ tiền) được gọi trực tiếp mà không có bước xác thực người dùng hoặc kiểm tra quyền ở đầu luồng.
   * **Cách kiểm tra**: Kiểm tra xem các hàm xử lý thay đổi trạng thái có các tin nhắn kiểm tra quyền hoặc xác thực (như `CheckPermission`, `Authorize`) đứng trước hay không.
   * **Giải pháp**: Yêu cầu chèn thêm bước kiểm tra Auth/Validation trước khi đi sâu vào luồng xử lý nghiệp vụ.

5. **Thiếu luồng xử lý lỗi (Missing Alternative Path)**:
   * **Dấu hiệu**: Sơ đồ chỉ mô tả luồng chạy thành công (Happy Path), hoàn toàn không chứa khung logic `alt` hoặc `opt` để xử lý các ngoại lệ (như mất kết nối, lỗi thanh toán, không tìm thấy thực thể).
   * **Cách kiểm tra**: Kiểm tra sự tồn tại của các thẻ phân đoạn logic (`alt`, `opt`) trong sơ đồ.
   * **Giải pháp**: Yêu cầu vẽ thêm khung phân đoạn `alt` để mô tả xử lý khi gặp sự cố kỹ thuật hoặc nghiệp vụ thất bại.

6. **Sai chiều và ký pháp mũi tên tin nhắn**:
   * **Quy tắc bắt buộc**: 
     - Tin nhắn gọi hàm (Sync/Async Call) đầu mũi tên trỏ từ đối tượng Gọi sang đối tượng Nhận.
     - Tin nhắn phản hồi (Return) **bắt buộc dùng nét đứt** (`-->>`) và đầu mũi tên trỏ ngược về phía Caller ban đầu.
     - Các Thanh kích hoạt (Activation Bars) phải thẳng hàng khớp với thời gian gọi phương thức.
   * **Cách kiểm tra**: Đọc các quan hệ tin nhắn trong JSON và đối chiếu nét vẽ/đầu mũi tên.
   * **Giải pháp**: Yêu cầu điều chỉnh lại nét đứt cho tin nhắn phản hồi và đảo chiều nếu vẽ ngược.