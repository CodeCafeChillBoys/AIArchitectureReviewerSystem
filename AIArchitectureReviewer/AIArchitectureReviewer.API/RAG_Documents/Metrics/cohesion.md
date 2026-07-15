# Cohesion

## Mô tả
Cohesion đo mức độ các phần bên trong một class, module, hoặc subsystem liên kết với nhau để cùng phục vụ một mục đích rõ ràng. Cohesion càng cao thì thành phần đó càng “đúng vai trò”, dễ hiểu, dễ test, và dễ thay đổi.

Một class có cohesion tốt thường có trách nhiệm tập trung, các method và field của nó cùng xoay quanh một khái niệm nhất quán.

## Ý tưởng cốt lõi
- Mỗi thành phần nên có một mục đích rõ ràng.
- Các phần bên trong nên liên quan chặt đến cùng một trách nhiệm.
- Cohesion cao thường là dấu hiệu của thiết kế tốt.

## Dấu hiệu nhận biết
- Method và field trong cùng class đều phục vụ một chủ đề chung.
- Module có ranh giới rõ ràng và dễ mô tả bằng một câu ngắn.
- Ít method phải dùng tới data không liên quan.
- Thay đổi ở một feature thường chỉ tác động một vùng nhỏ.

## Các mức thường gặp
- Functional cohesion: tất cả phần trong module cùng phục vụ một chức năng duy nhất.
- Sequential cohesion: output của bước trước là input của bước sau.
- Communicational cohesion: các phần làm việc trên cùng dữ liệu.
- Logical cohesion: các hành vi cùng nhóm theo loại nhưng không thật sự cùng mục tiêu.
- Coincidental cohesion: các phần bị gom chung một cách ngẫu nhiên, rất yếu.

## Tác hại khi cohesion thấp
- Class hoặc module khó hiểu vì chứa nhiều ý tưởng khác nhau.
- Test khó vì setup nhiều concern không liên quan.
- Dễ sinh God Class hoặc utility class phình to.
- Refactor về sau khó vì boundary đã mờ.

## Nguyên nhân thường gặp
- Gom tạm logic vào nơi tiện nhất.
- Thiếu domain model hoặc use case boundary rõ.
- Không tách được concerns ngay từ đầu.
- Reuse sai cách, biến một class thành “nồi lẩu”.

## Ví dụ tốt (Python)
```python
class Invoice:
    def __init__(self, items, tax_rate):
        self.items = items
        self.tax_rate = tax_rate

    def subtotal(self):
        return sum(item["price"] * item["quantity"] for item in self.items)

    def tax(self):
        return self.subtotal() * self.tax_rate

    def total(self):
        return self.subtotal() + self.tax()
```

Tất cả method trong `Invoice` đều xoay quanh cùng một khái niệm: tính tiền hóa đơn. Đây là cohesion tốt.

## Ví dụ xấu (Python)
```python
class MiscManager:
    def send_email(self, to, body): pass
    def calculate_tax(self, amount): pass
    def format_date(self, value): pass
    def save_user(self, user): pass
```

Class này chứa các trách nhiệm không liên quan, là dấu hiệu của cohesion thấp.

## Cách xử lý
- Tách class hoặc module theo trách nhiệm chính.
- Đưa behavior về đúng domain object.
- Chia service lớn thành use case nhỏ hơn.
- Nếu chỉ là helper thật sự, giữ helper đó thật nhỏ và rõ phạm vi.

## Hướng refactor thường dùng
- Extract Class.
- Extract Method.
- Move Method.
- Replace Large Class with Smaller Collaborators.
- Introduce Value Object hoặc Domain Service khi cần.

## Phân biệt nhanh
- Cohesion là mức tập trung bên trong một thành phần.
- Coupling là mức phụ thuộc giữa các thành phần.
- God Class thường vừa cohesion thấp vừa coupling cao.

## Checklist
- [ ] Class/module có một mục đích chính rõ ràng.
- [ ] Các method bên trong phục vụ cùng một trách nhiệm.
- [ ] Ít concern không liên quan bị nhét chung.
- [ ] Có thể mô tả class/module bằng một câu ngắn.
- [ ] Khi thay đổi một lý do, không làm nó “đổi nghĩa” hoàn toàn.

## Tóm tắt ngắn
Cohesion cao là dấu hiệu tốt: các phần bên trong cùng phục vụ một mục tiêu rõ ràng. Khi cohesion thấp, thành phần thường cần được tách nhỏ hơn.

---

Người soạn: ArchReview AI — Cohesion.