# Quy tắc Kiểm tra Tính Nhất quán Liên Sơ đồ (Cross-Diagram Consistency Rules)

Khi đối chiếu chéo nhiều sơ đồ khác nhau trong cùng một dự án, AI cần tuân thủ các quy tắc sau:

1. **Đối chiếu giữa Sơ đồ Tuần tự (Sequence Diagram) và Sơ đồ Lớp (Class Diagram)**:
   - Mọi thực thể/lifeline gửi hoặc nhận tin nhắn trong sơ đồ Sequence bắt buộc phải có một lớp (Class) tương ứng trong sơ đồ Lớp.
   - Khi thực thể A gọi một phương thức của thực thể B (ví dụ: `B.CreateOrder()`), lớp B trong sơ đồ Lớp bắt buộc phải có khai báo phương thức `CreateOrder` (khớp tên và tham số).

2. **Đối chiếu giữa Sơ đồ Tuần tự (Sequence Diagram) và Sơ đồ CSDL (ERD)**:
   - Mọi thao tác truy vấn, đọc, hoặc ghi dữ liệu trên Database/Repository trong sơ đồ Sequence phải tương ứng với một bảng (Table) thực tế tồn tại trong sơ đồ ERD.

3. **Đối chiếu giữa Sơ đồ Hoạt động (Flowchart) và Sơ đồ Tuần tự (Sequence Diagram)**:
   - Trình tự thực hiện các bước nghiệp vụ trong sơ đồ Hoạt động phải trùng khớp với trình tự thời gian gửi các tin nhắn trong sơ đồ Sequence (ví dụ: không được thanh toán trước khi xác thực).

4. **Đối chiếu giữa Sơ đồ Lớp (Class Diagram) và Sơ đồ CSDL (ERD)**:
   - Các lớp thực thể đại diện cho cơ sở dữ liệu (Entity Class) phải ánh xạ khớp với các bảng tương ứng trong ERD.
   - Các thuộc tính (attributes) trong Entity Class phải có cột (column) tương ứng có tên đồng nhất và kiểu dữ liệu tương thích trong ERD.