# Factory Method Pattern

## Mô tả
Factory Method là creational pattern tách logic khởi tạo object ra khỏi code sử dụng object đó. Thay vì client tự gọi constructor của concrete class, client làm việc với một creator abstraction và để subclass quyết định sẽ tạo object nào.

Nếu bạn cần tạo cả một họ sản phẩm liên quan, hãy xem thêm Abstract Factory trong file [abstract_factory.md](abstract_factory.md).

## Ý tưởng cốt lõi
- Đẩy quyết định tạo object xuống subclass.
- Giảm phụ thuộc vào concrete class.
- Cho phép mở rộng bằng cách thêm creator mới thay vì sửa client.

## Cấu trúc tham gia
- Product: interface/base class của object được tạo.
- Concrete Product: lớp cụ thể.
- Creator: khai báo factory method.
- Concrete Creator: quyết định trả về concrete product nào.
- Client: chỉ làm việc qua creator/product abstraction.

## Khi nào dùng
- Khi code cần tạo object nhưng không muốn biết lớp cụ thể.
- Khi muốn có nhiều biến thể khởi tạo khác nhau.
- Khi muốn mở rộng creation logic mà hạn chế sửa client.

## Khi không nên dùng
- Khi chỉ có một constructor đơn giản và không cần mở rộng.
- Khi việc trừu tượng hóa creation làm code phức tạp hơn lợi ích nhận được.

## Ví dụ (Python)
Ví dụ dưới đây mô tả việc tạo thông báo khác nhau tùy kênh gửi.

```python
from abc import ABC, abstractmethod


class Notification(ABC):
    @abstractmethod
    def send(self):
        pass


class EmailNotification(Notification):
    def send(self):
        return "send email"


class SmsNotification(Notification):
    def send(self):
        return "send sms"


class NotificationCreator(ABC):
    @abstractmethod
    def factory_method(self):
        pass

    def notify(self):
        notification = self.factory_method()
        return notification.send()


class EmailCreator(NotificationCreator):
    def factory_method(self):
        return EmailNotification()


class SmsCreator(NotificationCreator):
    def factory_method(self):
        return SmsNotification()


creator = EmailCreator()
print(creator.notify())
```

### Giải thích ví dụ
- `Notification` là product abstraction.
- `EmailNotification` và `SmsNotification` là concrete products.
- `NotificationCreator` định nghĩa khung xử lý và để `factory_method()` tạo product.
- `EmailCreator` và `SmsCreator` quyết định product cụ thể.

## Quy trình hoạt động
1. Client chọn concrete creator.
2. Client gọi method nghiệp vụ của creator.
3. Creator gọi factory method bên trong.
4. Concrete creator trả về product phù hợp.
5. Creator dùng product qua abstraction.

## Ưu điểm
- Tách creation và usage.
- Dễ thêm loại product mới bằng cách thêm creator/product mới.
- Client ít phụ thuộc vào concrete class.

## Nhược điểm
- Tăng số lượng class.
- Có thể làm hệ thống quá trừu tượng nếu dùng cho bài toán nhỏ.

## Phân biệt nhanh
- Factory Method tạo một object qua một method được override.
- Abstract Factory tạo cả một họ object liên quan.
- Builder tập trung vào quá trình dựng object theo bước.

## Lỗi thường gặp
- Đặt quá nhiều logic nghiệp vụ trong creator thay vì trong product hoặc service phù hợp.
- Viết factory method nhưng vẫn để client tự quyết định concrete class.
- Nhầm Factory Method với Abstract Factory.

## Checklist
- [ ] Client không khởi tạo concrete product trực tiếp.
- [ ] Creator có factory method rõ ràng.
- [ ] Có thể mở rộng loại product mới bằng subclass/implementation mới.

## Tóm tắt ngắn
Factory Method giúp tách logic tạo object khỏi nơi dùng object, rất phù hợp khi bạn muốn mở rộng cách khởi tạo mà không làm client phụ thuộc vào concrete class.

---

Người soạn: ArchReview AI — Factory Method pattern.