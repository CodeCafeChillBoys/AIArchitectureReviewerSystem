# Quy tắc Đánh giá Sơ đồ CSDL (ERD / Database Diagram)

Sơ đồ ERD mô tả cấu trúc lưu trữ dữ liệu của hệ thống. AI cần kiểm tra các quy tắc sau:

1. **Vi phạm chuẩn hóa dữ liệu (1NF, 2NF, 3NF)**:
   - 1NF: Bảng chứa cột có kiểu dữ liệu phức hợp (danh sách, mảng, chuỗi JSON phân tách bằng dấu phẩy).
   - 2NF & 3NF: Bảng chứa các cột dữ liệu không phụ thuộc hoàn toàn vào khóa chính hoặc phụ thuộc bắc cầu (gây dư thừa dữ liệu).

2. **Mối quan hệ Nhiều-Nhiều (Many-to-Many) chưa được chuẩn hóa**:
   - Dấu hiệu: Hai bảng liên kết trực tiếp N-N với nhau mà không qua bảng trung gian (Junction/Association Table) để lưu khóa ngoại.

3. **Thiếu ràng buộc Khóa ngoại (Missing Foreign Key)**:
   - Dấu hiệu: Có quan hệ logic giữa hai bảng nhưng bảng con thiếu cột khóa ngoại trỏ về bảng cha để thiết lập ràng buộc toàn vẹn dữ liệu.

4. **Dư thừa dữ liệu (Data Redundancy)**:
   - Dấu hiệu: Trùng lặp các trường thông tin có thể suy ra từ bảng khác hoặc tính toán được.

5. **Ký hiệu/hướng quan hệ đầu cardinalities**:
   - Đầu khóa ngoại (bảng con - Nhiều) phải trỏ về bảng chứa khóa chính (bảng cha - 1). Ký hiệu cardinality phải chính xác.