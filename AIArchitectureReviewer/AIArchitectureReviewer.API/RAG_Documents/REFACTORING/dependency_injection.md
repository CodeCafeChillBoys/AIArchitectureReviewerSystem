# Dependency Injection (DI) — Tiêm phụ thuộc

## Mô tả
Dependency Injection là kỹ thuật cung cấp các phụ thuộc (dependencies) cho một đối tượng thay vì để đối tượng tự khởi tạo chúng. Thường sử dụng constructor injection, setter injection hoặc container-based injection.

## Lợi ích
- Dễ test (có thể inject mock)
- Giảm coupling, tăng khả năng thay đổi implement
- Hỗ trợ cấu hình linh hoạt (ví dụ: đổi DB provider)

## Các hình thức
- Constructor Injection: phụ thuộc được truyền qua constructor.
- Setter Injection: phụ thuộc được gán qua setter.
- Interface Injection: ít dùng — object implement interface để nhận phụ thuộc.

## Ví dụ (Python)
```python
class EmailService:
    def send(self, to, body): pass

class NotificationService:
    def __init__(self, email_service: EmailService):
        self.email = email_service
    def notify(self, user, msg):
        self.email.send(user.email, msg)

# Runtime
# notification = NotificationService(SmtpEmailService())
# Test
# notification = NotificationService(MockEmailService())
```

## Checklist
- [ ] Có thể inject mock trong test không?
- [ ] High-level module phụ thuộc vào abstraction chứ không phải chi tiết?
- [ ] Có DI container hay factory để cấu hình implement khi cần?

## Lưu ý
- DI làm tăng số lượng lớp/interface, cân nhắc trade-off.
- Dùng DI container khi project lớn; đơn giản thì dùng constructor injection.

---

Người soạn: ArchReview AI — Hướng dẫn Dependency Injection.