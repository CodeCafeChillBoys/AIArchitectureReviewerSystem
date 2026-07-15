# ERD (Entity-Relationship Diagram)

## Mô tả
ERD là biểu diễn đồ họa các thực thể trong hệ thống và các quan hệ giữa chúng. ERD giúp chuyển domain model sang schema quan hệ và truyền đạt cấu trúc dữ liệu cho nhóm phát triển.

## Các thành phần chính
- Entity: thực thể (table) chứa attribute.
- Attribute: cột của entity (có thể là PK, FK, hoặc non-key).
- Relationship: mối quan hệ giữa entities (1:1, 1:N, N:M).
- Cardinality & Participation: chỉ rõ số lượng và tính bắt buộc của mối quan hệ.

## Khi nào dùng
- Phase design trước khi implement database.
- Kiểm tra consistency giữa domain model và schema.

## Công cụ vẽ ERD
- Draw.io, Lucidchart, dbdiagram.io, MySQL Workbench, pgAdmin.

## Ví dụ mẫu
Entities: `Customer`, `Order`, `Product`, `OrderItem`.
Relationships:
- Customer 1---* Order
- Order 1---* OrderItem
- Product 1---* OrderItem

SQL DDL (ví dụ PostgreSQL):

CREATE TABLE customers (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    address TEXT
);

CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    price NUMERIC(10,2) NOT NULL
);

CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER REFERENCES customers(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE order_items (
    order_id INTEGER REFERENCES orders(id) ON DELETE CASCADE,
    product_id INTEGER REFERENCES products(id),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (order_id, product_id)
);

## Checklist
- [ ] Tất cả PKs đã xác định.
- [ ] FK và ràng buộc đã thêm.
- [ ] Cardinality rõ ràng giữa entities.
- [ ] Tối ưu hóa cho truy vấn phổ biến (indexing).
- [ ] Xem xét cascading rules (ON DELETE/UPDATE).

## Best practices
- Sử dụng surrogate keys khi phù hợp.
- Thêm unique constraints cho các business keys.
- Index cho các cột FK và cột truy vấn thường xuyên.
- Document giả định về nullability và default values.

## Tóm tắt ngắn
ERD giúp truyền đạt cấu trúc dữ liệu, phát hiện thiếu sót sớm và tạo tiền đề cho việc viết DDL chính xác.

---

Người soạn: ArchReview AI — ERD.