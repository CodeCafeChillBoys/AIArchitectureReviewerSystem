# Repository Pattern

## Mô tả
Repository pattern tách biệt logic truy cập dữ liệu khỏi business logic bằng một lớp trung gian (repository) cung cấp API cho các thao tác CRUD. Nó giúp viết code testable, thay đổi dễ dàng giữa các nguồn dữ liệu, và giữ domain layer sạch.

## Ý tưởng cốt lõi
- Tạo abstraction cho persistence (CRUD, query methods).
- Business logic tương tác với repository thay vì trực tiếp gọi ORM/SQL.
- Hỗ trợ mocking và unit testing dễ dàng.

## Khi nào dùng
- Ứng dụng với business logic phức tạp và cần test isolation.
- Khi muốn tách persistency concern, hoặc hỗ trợ nhiều nguồn dữ liệu.

## Khi không dùng
- Ứng dụng nhỏ, simple scripts, hoặc nơi ORM query DSL đủ rõ ràng.
- Khi repository chỉ là thin wrapper quanh ORM mà không đóng góp abstraction value.

## Ví dụ (Python, using SQLAlchemy)

class ProductRepository:
    def __init__(self, session):
        self.session = session

    def get_by_id(self, product_id):
        return self.session.query(Product).filter_by(id=product_id).one_or_none()

    def list(self, limit=100, offset=0):
        return self.session.query(Product).limit(limit).offset(offset).all()

    def add(self, product):
        self.session.add(product)
        self.session.commit()
        return product

    def remove(self, product_id):
        obj = self.get_by_id(product_id)
        if obj:
            self.session.delete(obj)
            self.session.commit()

## Patterns & variations
- Generic repository vs specific repository per aggregate root.
- Unit of Work integration for transaction management.
- Query objects / Specification pattern for complex filtering.

## Checklist
- [ ] Define repository interfaces for domain operations.
- [ ] Keep transactions at service/unit-of-work level.
- [ ] Avoid leaking ORM-specific types through repository API.
- [ ] Provide pagination/sorting/filtering primitives.
- [ ] Add tests that mock repositories for unit tests.

## Best practices
- Prefer specific repositories for aggregate roots.
- Keep mapping logic (DTO <-> Entity) outside repository or in thin mappers.
- Use UoW for atomic operations across multiple repositories.
- Keep repository surface small and intention-revealing.

## Tóm tắt ngắn
Repository pattern improves testability and separation of concerns but must be used where it adds real abstraction value, not just as an unnecessary wrapper.

---

Người soạn: ArchReview AI — Repository Pattern.