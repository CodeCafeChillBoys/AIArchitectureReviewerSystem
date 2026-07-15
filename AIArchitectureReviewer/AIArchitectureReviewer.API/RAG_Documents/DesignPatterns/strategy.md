# Strategy Pattern

## Mô tả
Strategy là behavioral pattern định nghĩa một họ các thuật toán, đóng gói từng thuật toán riêng biệt và cho phép thay thế chúng tại runtime. Context không cần biết chi tiết thuật toán đang dùng; nó chỉ gọi qua interface Strategy.

## Ý tưởng cốt lõi
- Tách thuật toán khỏi nơi sử dụng.
- Hoán đổi hành vi mà không sửa context.
- Tránh chuỗi `if/elif/else` phình to theo loại.

## Cấu trúc tham gia
- Strategy: interface của thuật toán.
- Concrete Strategy: các thuật toán cụ thể.
- Context: giữ reference tới strategy và dùng nó.
- Client: chọn strategy phù hợp.

## Khi nào dùng
- Khi cần đổi thuật toán tại runtime.
- Khi có nhiều biến thể xử lý cùng một bài toán.
- Khi muốn thay điều kiện rẽ nhánh bằng composition.

## Khi không nên dùng
- Khi chỉ có một thuật toán duy nhất.
- Khi việc trừu tượng hóa làm code khó hiểu hơn lợi ích nhận được.

## Ví dụ (Python)
Ví dụ dưới đây chọn chiến lược giảm giá khác nhau cho đơn hàng.

```python
from abc import ABC, abstractmethod


class DiscountStrategy(ABC):
    @abstractmethod
    def apply(self, amount):
        pass


class NoDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount


class BlackFridayDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.7


class MemberDiscount(DiscountStrategy):
    def apply(self, amount):
        return amount * 0.85


class Checkout:
    def __init__(self, strategy):
        self.strategy = strategy

    def total(self, amount):
        return self.strategy.apply(amount)


checkout = Checkout(BlackFridayDiscount())
print(checkout.total(100))
```

### Giải thích ví dụ
- `DiscountStrategy` là interface.
- Mỗi concrete strategy đóng gói một cách tính khác nhau.
- `Checkout` chỉ biết gọi strategy hiện tại.

## Quy trình hoạt động
1. Client chọn strategy.
2. Client truyền strategy vào context.
3. Context gọi strategy qua interface chung.
4. Kết quả trả về theo thuật toán đã chọn.

## Ưu điểm
- Dễ thay đổi thuật toán.
- Giảm các nhánh điều kiện phức tạp.
- Tăng tính mở rộng và dễ test từng thuật toán.

## Nhược điểm
- Tăng số lượng class.
- Client phải biết hoặc chọn strategy phù hợp.
- Nếu strategy quá nhỏ, code có thể bị phân mảnh.

## Phân biệt nhanh
- Strategy thay đổi thuật toán.
- Command đóng gói hành động để thực thi sau.
- State trông giống Strategy nhưng thay đổi hành vi dựa trên trạng thái nội bộ của context.

## Lỗi thường gặp
- Đẩy quá nhiều logic chọn strategy vào context.
- Tạo strategy nhưng không thật sự có nhiều biến thể.
- Nhầm Strategy với State khi hành vi phụ thuộc vào vòng đời trạng thái.

## Checklist
- [ ] Có nhiều thuật toán thay thế cho cùng một nhiệm vụ.
- [ ] Context chỉ phụ thuộc vào interface strategy.
- [ ] Strategy có thể thay đổi mà không sửa context.

## Tóm tắt ngắn
Strategy giúp thay đổi thuật toán một cách linh hoạt bằng composition thay vì nhồi thêm nhánh điều kiện trong context.

---

Người soạn: ArchReview AI — Strategy pattern.