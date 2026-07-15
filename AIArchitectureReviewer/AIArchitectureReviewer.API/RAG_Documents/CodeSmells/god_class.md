# God Class

## Mô tả
God Class là một class ôm quá nhiều trách nhiệm, quá nhiều state, hoặc quá nhiều logic nghiệp vụ đến mức trở thành trung tâm của hệ thống. Class này thường biết quá nhiều, làm quá nhiều, và phụ thuộc vào quá nhiều phần khác.

Đây là một trong những code smell nguy hiểm nhất vì nó làm giảm khả năng hiểu, test, sửa, và mở rộng hệ thống. God Class thường xuất hiện dần dần khi một class được chọn làm nơi đặt logic tạm thời rồi ngày càng phình ra.

## Ý tưởng cốt lõi
- Một class phải có một lý do thay đổi rõ ràng.
- Nếu class chứa quá nhiều chức năng không liên quan, nó đang vi phạm SRP.
- Khi một class trở thành điểm hội tụ của hầu hết logic, nó sẽ trở thành bottleneck của thiết kế.

## Dấu hiệu nhận biết
- Class có rất nhiều method và field.
- Class được gọi từ rất nhiều nơi.
- Class chứa nhiều khối `if/elif/else` theo loại nghiệp vụ khác nhau.
- Class vừa xử lý dữ liệu, vừa validate, vừa tính toán, vừa gọi DB, vừa gửi mail.
- Unit test cho class trở nên dài, khó setup, và khó mock.

## Tác hại
- Khó hiểu và khó onboarding.
- Dễ gây xung đột khi nhiều người cùng sửa.
- Thay đổi nhỏ có thể ảnh hưởng nhiều hành vi khác nhau.
- Test khó vì dependency dày đặc.
- Rất dễ làm phát sinh lỗi dây chuyền.

## Nguyên nhân thường gặp
- Thiếu ranh giới domain rõ ràng.
- Đẩy tạm logic vào một class tiện nhất.
- Không có kiến trúc phân tầng hoặc module boundary tốt.
- Refactor chậm, để class phình to theo thời gian.

## Ví dụ xấu (Python)
```python
class OrderManager:
    def __init__(self, repo, mailer, payment_gateway, logger):
        self.repo = repo
        self.mailer = mailer
        self.payment_gateway = payment_gateway
        self.logger = logger

    def create_order(self, data):
        self.logger.info("creating order")
        order = self._build_order(data)
        self._validate(order)
        self._charge(order)
        self.repo.save(order)
        self._send_confirmation(order)
        return order

    def cancel_order(self, order_id):
        order = self.repo.get(order_id)
        self._refund(order)
        order.status = "cancelled"
        self.repo.save(order)
        self._send_cancellation(order)

    def _build_order(self, data): pass
    def _validate(self, order): pass
    def _charge(self, order): pass
    def _refund(self, order): pass
    def _send_confirmation(self, order): pass
    def _send_cancellation(self, order): pass
```

## Cách xử lý
- Tách class theo trách nhiệm: service, validator, gateway, notifier, repository.
- Đưa rules nghiệp vụ vào domain object hoặc domain service.
- Chia logic theo use case thay vì gom vào một manager tổng.
- Tạo abstraction rõ ràng cho dependency ngoài.
- Áp dụng refactoring nhỏ và an toàn thay vì viết lại toàn bộ.

## Hướng refactor thường dùng
- Extract Class: tách các phần logic có trách nhiệm riêng.
- Extract Method: chia method quá dài thành bước nhỏ.
- Move Method: chuyển hành vi về nơi phù hợp hơn.
- Introduce Facade hoặc Application Service nếu class đang làm quá nhiều orchestration.
- Introduce Domain Service nếu logic nghiệp vụ không thuộc riêng entity nào.

## Phân biệt nhanh
- God Class là class ôm quá nhiều trách nhiệm.
- Anemic Domain Model là domain object thiếu behavior.
- Large Class có thể chỉ lớn về kích thước, còn God Class lớn cả về trách nhiệm và coupling.

## Checklist
- [ ] Class có hơn một lý do thay đổi rõ ràng.
- [ ] Class đang xử lý nhiều concern khác nhau.
- [ ] Có thể tách ra thành nhiều class nhỏ hơn.
- [ ] Test của class quá khó setup vì nhiều dependency.
- [ ] Thay đổi nhỏ cũng buộc phải sửa class này.

## Tóm tắt ngắn
God Class là tín hiệu rằng logic đã bị gom quá mức vào một điểm. Cách xử lý đúng là chia trách nhiệm, giới hạn coupling, và đẩy logic về đúng lớp phù hợp.

---

Người soạn: ArchReview AI — God Class.