# SOLID — Tổng quan

Thư mục này chứa tài liệu chi tiết cho 5 nguyên lý SOLID trong thiết kế hướng đối tượng.

## Mục lục
- SRP - Single Responsibility Principle — [srp.md](srp.md)
- OCP - Open/Closed Principle — [ocp.md](ocp.md)
- LSP - Liskov Substitution Principle — [lsp.md](lsp.md)
- ISP - Interface Segregation Principle — [isp.md](isp.md)
- DIP - Dependency Inversion Principle — [dip.md](dip.md)

## Cách dùng
- Mở từng file để xem mô tả, dấu hiệu vi phạm, cách refactor, ví dụ, checklist, và UML/diagram nếu có.
- Dùng như tài liệu tham khảo khi review code hoặc thiết kế kiến trúc.
- Các nguyên lý này thường đi cùng nhau và nên được xem như một bộ tiêu chí thiết kế.

## Gợi ý đọc nhanh
- Bắt đầu với SRP để xác định trách nhiệm.
- Dùng OCP để xem code có dễ mở rộng không.
- Kiểm tra LSP và ISP để đánh giá hợp đồng và interface.
- Kết thúc bằng DIP để xem hướng phụ thuộc có hợp lý hay không.

Mục tiêu của thư mục này là giúp nhận diện sớm vấn đề thiết kế trước khi chúng gây ra coupling cao, logic phân tán, và code khó bảo trì.