# Coupling

## Mô tả
Coupling đo mức độ phụ thuộc giữa các module, class, package, hoặc service. Coupling càng cao thì thay đổi ở một nơi càng dễ lan sang nơi khác, làm hệ thống khó sửa, khó test, và khó tái sử dụng.

Không phải mọi coupling đều xấu. Một mức độ phụ thuộc nhất định là cần thiết để hệ thống hoạt động. Vấn đề nằm ở coupling quá chặt, quá nhiều chiều, hoặc phụ thuộc vào chi tiết thay vì abstraction.

## Ý tưởng cốt lõi
- Hai thành phần càng phụ thuộc chặt thì càng khó thay đổi độc lập.
- Coupling nên được quản lý ở mức cần thiết, không nên loại bỏ hoàn toàn.
- Phụ thuộc vào abstraction thường tốt hơn phụ thuộc vào concrete implementation.

## Dấu hiệu nhận biết
- Sửa một class nhưng nhiều class khác cũng phải sửa theo.
- Module import trực tiếp nhau theo nhiều chiều.
- Class biết quá nhiều chi tiết nội bộ của class khác.
- Service gọi chéo nhau dày đặc.
- Test khó vì phải dựng quá nhiều dependency thật.

## Các dạng coupling thường gặp
- Content coupling: một module đụng vào chi tiết nội bộ của module khác.
- Common coupling: nhiều nơi chia sẻ global state.
- Control coupling: truyền flag điều khiển logic nội bộ của module khác.
- Stamp coupling: truyền cả object lớn chỉ để dùng một phần nhỏ.
- Data coupling: trao đổi dữ liệu tối thiểu cần thiết, thường là mức mong muốn hơn.

## Tác hại
- Làm thay đổi lan rộng.
- Tăng rủi ro bug khi refactor.
- Khó unit test và mock.
- Làm kiến trúc cứng, kém linh hoạt.
- Gây khó khăn khi tách module hoặc service sau này.

## Nguyên nhân thường gặp
- Thiết kế boundary không rõ.
- Lạm dụng global state hoặc singleton.
- Gọi trực tiếp concrete class thay vì interface.
- Để module biết quá nhiều về cấu trúc nội bộ của nhau.
- Trộn nhiều trách nhiệm trong một class hoặc service.

## Ví dụ xấu (Python)
```python
class OrderService:
    def __init__(self, payment_gateway, inventory_service, notification_service):
        self.payment_gateway = payment_gateway
        self.inventory_service = inventory_service
        self.notification_service = notification_service

    def checkout(self, order):
        self.inventory_service.reserve(order.items)
        self.payment_gateway.charge(order.total)
        self.notification_service.send(order.user, "paid")
```

Ví dụ này chưa chắc xấu nếu đây là orchestration có chủ đích. Nhưng coupling sẽ trở thành vấn đề nếu `OrderService` biết quá nhiều chi tiết của từng dependency, gọi field nội bộ, hoặc phải sửa liên tục mỗi khi dependency đổi.

## Cách xử lý
- Phụ thuộc vào interface hoặc port thay vì concrete implementation.
- Giảm số dependency mà một class/service cần.
- Dùng event hoặc message để thay cho gọi trực tiếp khi phù hợp.
- Tách orchestration logic ra khỏi low-level details.
- Chia boundary theo domain hoặc use case rõ ràng.

## Hướng refactor thường dùng
- Dependency Inversion.
- Introduce Interface / Port.
- Extract Class.
- Facade hoặc Mediator để giảm phụ thuộc chéo.
- Event-driven integration cho các luồng không cần đồng bộ tức thì.

## Phân biệt nhanh
- Coupling là mức phụ thuộc giữa các thành phần.
- Cohesion là mức tập trung trách nhiệm bên trong một thành phần.
- Cyclic Dependency là một dạng coupling xấu có vòng lặp.

## Checklist
- [ ] Thành phần có phụ thuộc vào abstraction thay vì concrete class không.
- [ ] Thay đổi ở một nơi có lan sang nhiều nơi không.
- [ ] Có global state hoặc shared mutable state không.
- [ ] Có thể giảm số dependency trực tiếp không.
- [ ] Có vòng phụ thuộc giữa các module không.

## Tóm tắt ngắn
Coupling càng cao thì hệ thống càng khó thay đổi độc lập. Mục tiêu không phải xóa coupling, mà là giữ nó ở mức vừa đủ và đúng chỗ.

---

Người soạn: ArchReview AI — Coupling.