---

### `01_SOLID/ocp.md`

# Open/Closed Principle (OCP) - Nguyên lý Đóng/Mở

## Tổng quan
Nguyên lý Open/Closed (OCP) nói rằng: "Các thực thể phần mềm (lớp, module, hàm) nên mở rộng được (open for extension) nhưng đóng để sửa đổi (closed for modification)." Nói cách khác, khi muốn thêm tính năng hoặc hành vi mới, ta nên thêm mã mới (mở rộng), không chỉnh sửa mã đã hoạt động.

Mục tiêu của OCP là giảm rủi ro khi thay đổi, tăng khả năng bảo trì và mở rộng hệ thống mà không phá vỡ chức năng hiện có.

## Khi nào cần áp dụng
- Khi hệ thống phải tiếp tục mở rộng tính năng theo thời gian.
- Khi thay đổi trong tương lai có khả năng gây lỗi cho mã hiện có.
- Khi bạn thấy các `if-else`/`switch` phân nhánh theo loại hoặc hành vi.

## Dấu hiệu vi phạm (Code smells)
- Chuỗi `if-else` hoặc `switch-case` dựa trên giá trị kiểu hoặc tag.
- Thay đổi yêu cầu luôn làm sửa các hàm cốt lõi.
- Có nhiều điều kiện phân nhánh theo loại đối tượng.

## Cách tiếp cận chung
- Trừu tượng hóa (interfaces/abstract classes).
- Sử dụng đa hình (polymorphism) thay vì kiểm tra kiểu.
- Áp dụng các design patterns: Strategy, State, Factory Method, Visitor (cẩn trọng), Extension Objects.
- Tách điểm thay đổi (separation of concerns) và đóng gói hành vi có thể thay đổi.

## Ví dụ thực tế — Sai (vi phạm OCP) — Python
```python
class PaymentProcessor:
    def process(self, payment_type: str, amount: float):
        if payment_type == 'credit_card':
            print('Process credit card', amount)
        elif payment_type == 'paypal':
            print('Process PayPal', amount)
        # Nếu thêm 'momo', phải sửa hàm này -> vi phạm OCP


proc = PaymentProcessor()
proc.process('paypal', 10.0)
```

Vấn đề: mỗi lần có phương thức thanh toán mới, bạn phải sửa `process`.

## Ví dụ đúng (tuân OCP) — Python (Strategy pattern)
```python
from abc import ABC, abstractmethod

class PaymentStrategy(ABC):
    @abstractmethod
    def pay(self, amount: float):
        pass

class CreditCardPayment(PaymentStrategy):
    def pay(self, amount: float):
        print('Processing credit card:', amount)

class PayPalPayment(PaymentStrategy):
    def pay(self, amount: float):
        print('Processing PayPal:', amount)

class PaymentProcessor:
    def __init__(self, strategy: PaymentStrategy):
        self._strategy = strategy

    def process(self, amount: float):
        self._strategy.pay(amount)

# Thêm phương thức mới không cần sửa PaymentProcessor
processor = PaymentProcessor(PayPalPayment())
processor.process(20.0)
```

## Ví dụ Java (tập trung interface + factory)
```java
public interface Payment {
    void pay(double amount);
}

public class CreditCard implements Payment {
    public void pay(double amount) { System.out.println("Credit: " + amount); }
}

public class PayPal implements Payment {
    public void pay(double amount) { System.out.println("PayPal: " + amount); }
}

// Factory để tạo Payment theo config — mở rộng bằng cách thêm class mới
public class PaymentFactory {
    public static Payment create(String type) {
        switch(type) {
            case "credit": return new CreditCard();
            case "paypal": return new PayPal();
            default: throw new IllegalArgumentException("Unknown type");
        }
    }
}

// Consumer không cần thay đổi khi thêm phương thức mới (chỉ cần update factory hoặc dùng DI)
```

> Ghi chú: để tuân thủ OCP tuyệt đối, tránh switch trong factory bằng cách đăng ký kiểu động (registry) hoặc dependency injection.

## Refactor từ vi phạm sang tuân OCP — các bước thực tế
1. Xác định điểm biến đổi (hotspot) — nơi thường xuyên bị thay đổi.
2. Trích xuất interface hoặc abstract class cho hành vi thay đổi.
3. Di chuyển các nhánh `if` thành các lớp cụ thể (concrete classes) triển khai interface.
4. Thêm factory hoặc dependency injection để khởi tạo thể hiện phù hợp.
5. Viết test cho interface và các implement để đảm bảo hành vi.

## Anti-patterns liên quan
- God Object: lớp thực hiện quá nhiều trách nhiệm và chứa nhiều điều kiện.
- Switch/If chaining: cấu trúc điều kiện lớn thay vì đa hình.
- Rigid architecture: mọi thay đổi bắt buộc sửa cùng một file.

## Checklist kiểm tra tuân OCP
- [ ] Có Class/Module chịu trách nhiệm cho hành vi thay đổi?
- [ ] Có interface/abstraction rõ ràng cho hành vi đó?
- [ ] Thêm tính năng mới có gây sửa code hiện có không?
- [ ] Có test bảo đảm các trường hợp cũ không bị hỏng khi thêm?
- [ ] Sử dụng DI/factory để cấu hình implement cụ thể không phải sửa consumer?

## Ví dụ refactor nhỏ (từ if-else sang đa hình) — Python
```python
# Before: (vi phạm)
def area(shape):
    if shape['type'] == 'circle':
        return 3.14 * shape['r'] ** 2
    elif shape['type'] == 'rectangle':
        return shape['w'] * shape['h']

# After: (tuân OCP)
from abc import ABC, abstractmethod

class Shape(ABC):
    @abstractmethod
    def area(self):
        pass

class Circle(Shape):
    def __init__(self, r): self.r = r
    def area(self): return 3.14 * self.r ** 2

class Rectangle(Shape):
    def __init__(self, w, h): self.w, self.h = w, h
    def area(self): return self.w * self.h

def total_area(shapes):
    return sum(s.area() for s in shapes)
```

## UML / Diagram (Mermaid)
```mermaid
classDiagram
    class PaymentStrategy {
        <<interface>>
        +pay(amount)
    }
    class CreditCardPayment {
        +pay(amount)
    }
    class PayPalPayment {
        +pay(amount)
+        
*** End Patch