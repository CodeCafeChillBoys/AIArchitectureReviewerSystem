# Observer Pattern

## Mô tả
Observer là behavioral pattern thiết lập quan hệ one-to-many giữa một subject và nhiều observers. Khi trạng thái của subject thay đổi, subject sẽ thông báo cho tất cả observers đã đăng ký.

Pattern này là nền tảng của event-driven programming, publish-subscribe, listener, và callback-based design.

## Ý tưởng cốt lõi
- Subject giữ danh sách observer.
- Observer đăng ký hoặc hủy đăng ký động.
- Subject phát thông báo khi có thay đổi.
- Observers phản ứng độc lập với nhau.

## Cấu trúc tham gia
- Subject: quản lý danh sách observer.
- Observer: interface nhận update.
- Concrete Subject: giữ trạng thái thực.
- Concrete Observer: xử lý update theo nhu cầu.

## Khi nào dùng
- Khi cần thông báo một thay đổi cho nhiều nơi cùng lúc.
- Khi muốn tách nguồn sự kiện khỏi nơi xử lý.
- Khi muốn giảm coupling giữa producer và consumers.

## Khi không nên dùng
- Khi chỉ có một consumer duy nhất và gọi trực tiếp đơn giản hơn.
- Khi cần đảm bảo thứ tự phức tạp hoặc transaction chặt chẽ, vì observer có thể làm luồng xử lý khó kiểm soát.

## Ví dụ (Python)
Ví dụ dưới đây mô phỏng hệ thống giá cổ phiếu thông báo cho các subscriber.

```python
from abc import ABC, abstractmethod


class Observer(ABC):
    @abstractmethod
    def update(self, subject):
        pass


class StockSubject:
    def __init__(self):
        self._observers = []
        self._price = 0

    def attach(self, observer):
        self._observers.append(observer)

    def detach(self, observer):
        self._observers.remove(observer)

    def set_price(self, price):
        self._price = price
        self.notify()

    def notify(self):
        for observer in self._observers:
            observer.update(self)

    @property
    def price(self):
        return self._price


class ConsoleObserver(Observer):
    def __init__(self, name):
        self.name = name

    def update(self, subject):
        print(f"{self.name} received price: {subject.price}")


stock = StockSubject()
stock.attach(ConsoleObserver("UI"))
stock.attach(ConsoleObserver("Logger"))
stock.set_price(120)
```

### Giải thích ví dụ
- `StockSubject` là subject.
- `ConsoleObserver` là concrete observer.
- Khi `set_price()` thay đổi trạng thái, subject gọi `notify()`.

## Quy trình hoạt động
1. Observer đăng ký với subject.
2. Subject thay đổi trạng thái.
3. Subject gọi `notify()`.
4. Mỗi observer nhận update và xử lý riêng.

## Ưu điểm
- Giảm coupling giữa producer và consumer.
- Dễ thêm observer mới mà không sửa subject.
- Phù hợp cho event-driven architecture.

## Nhược điểm
- Khó debug khi có nhiều observer.
- Có thể gây cascade update hoặc performance issues nếu danh sách quá lớn.
- Thứ tự notify có thể ảnh hưởng kết quả.

## Phân biệt nhanh
- Observer phát thông báo cho nhiều subscriber.
- Command đóng gói hành động thành object.
- Strategy thay đổi thuật toán được sử dụng.

## Lỗi thường gặp
- Quên detach observer không còn dùng, dễ gây leak.
- Để observer phụ thuộc vào quá nhiều chi tiết của subject.
- Trigger notify quá thường xuyên cho thay đổi không cần thiết.

## Checklist
- [ ] Subject quản lý observer rõ ràng.
- [ ] Observer có thể thêm/bớt động.
- [ ] Update của observer độc lập với nhau.
- [ ] Có cơ chế detach khi observer không còn sống.

## Tóm tắt ngắn
Observer giúp tách source của sự kiện khỏi các phần phản ứng với sự kiện, rất phù hợp cho hệ thống cần broadcast thay đổi tới nhiều nơi.

---

Người soạn: ArchReview AI — Observer pattern.