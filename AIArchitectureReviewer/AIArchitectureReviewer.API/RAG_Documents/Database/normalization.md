# Database Normalization

## Mô tả
Normalization là quá trình tổ chức dữ liệu trong cơ sở dữ liệu quan hệ để giảm dư thừa và tránh các vấn đề về cập nhật (update anomalies), chèn (insert anomalies), và xóa (delete anomalies). Mục tiêu là đảm bảo tính toàn vẹn dữ liệu và tối ưu hóa cấu trúc bảng.

## Các dạng chuẩn (normal forms)
- 1NF (First Normal Form): mỗi ô chứa giá trị nguyên tố (atomic), không có các nhóm giá trị lồng.
- 2NF (Second Normal Form): thỏa 1NF và mọi thuộc tính không chính phụ thuộc hoàn toàn vào khóa chính (no partial dependency).
- 3NF (Third Normal Form): thỏa 2NF và không có phụ thuộc bắc cầu (no transitive dependency) từ non-key tới non-key.
- BCNF (Boyce–Codd NF): mạnh hơn 3NF, mọi determinant phải là candidate key.
- 4NF, 5NF: xử lý multivalued dependencies và join dependencies cho các trường hợp phức tạp hơn.

## Khi nào áp dụng
- Thiết kế schema từ domain model.
- Khi cần đảm bảo tính nhất quán dữ liệu và tránh anomalies.
- Khi dung lượng dư thừa hoặc inconsistency xuất hiện.

## Khi không nên áp dụng quá mức
- Kiến trúc read-heavy với nhiều join có thể làm giảm hiệu năng.
- Các hệ thống OLAP, data warehouse thường dùng denormalized schemas (star/snowflake) để tối ưu truy vấn analytic.
- Microservices có database per service: duplication có thể chấp nhận nếu giúp autonomy.

## Ví dụ (từ 1NF -> 3NF)
Giả sử data ban đầu: `Order(order_id, customer_name, customer_address, product_id, product_name, quantity)`

- 1NF: tách từng giá trị atomic (không lưu list trong một cột).
- 2NF: tách `Order` thành `Orders(order_id, customer_id)` và `OrderItems(order_id, product_id, quantity)`, `Customers(customer_id, name, address)`.
- 3NF: đảm bảo `product_name` không phụ thuộc transitively trên `order_id`; `Products(product_id, name)`.

## Checklist
- [ ] Xác định khóa chính và khóa ứng viên.
- [ ] Kiểm tra partial dependency và tách bảng nếu cần.
- [ ] Kiểm tra transitive dependency.
- [ ] Cân nhắc performance trade-offs trước khi denormalize.
- [ ] Document các quyết định denormalize (caching, materialized views).

## Best practices
- Bắt đầu bằng 3NF cho OLTP.
- Dùng views/materialized views hoặc caches cho truy vấn nặng.
- Trong microservices, không ngại một số duplication để đảm bảo autonomy.
- Sử dụng migrations và scripts để duy trì schema changes.

## Tóm tắt ngắn
Normalization giúp loại bỏ dư thừa và anomalies, nhưng cần cân nhắc trade-offs về hiệu năng và tính năng hệ thống khi quyết định denormalize.

---

Người soạn: ArchReview AI — Normalization.