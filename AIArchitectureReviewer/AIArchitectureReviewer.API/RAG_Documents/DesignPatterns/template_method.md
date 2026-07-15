# Template Method Pattern

## Mô tả
Template Method là behavioral pattern định nghĩa khung sườn của một thuật toán trong superclass và cho phép subclass ghi đè một số bước cụ thể mà không làm thay đổi trình tự tổng thể.

Pattern này phù hợp khi các biến thể của cùng một quy trình chia sẻ phần lớn logic chung nhưng khác nhau ở một vài bước nhỏ.

## Ý tưởng cốt lõi
- Superclass giữ thuật toán tổng thể.
- Subclass chỉ thay đổi các bước được cho phép.
- Cấu trúc chính của luồng xử lý được cố định.

## Cấu trúc tham gia
- Abstract Class / Base Class: định nghĩa template method.
- Concrete Class: triển khai các bước cụ thể.
- Template Method: method điều phối toàn bộ thuật toán.

## Khi nào dùng
- Khi nhiều class chia sẻ cùng một quy trình tổng thể.
- Khi muốn tái sử dụng logic chung trong superclass.
- Khi muốn kiểm soát thứ tự các bước xử lý.

## Khi không nên dùng
- Khi các bước không có phần chung rõ ràng.
- Khi nên dùng composition/strategy thay vì kế thừa.
- Khi subclass hóa làm hệ thống cứng và khó mở rộng.

## Ví dụ (Python)
Ví dụ dưới đây mô tả một trò chơi có quy trình khởi động và chơi chung, nhưng từng trò chơi cụ thể triển khai chi tiết khác nhau.

```python
from abc import ABC, abstractmethod


class Game(ABC):
    def play(self):
        self.setup()
        self.take_turns()
        self.tear_down()

    def setup(self):
        pass

    @abstractmethod
    def take_turns(self):
        pass

    def tear_down(self):
        pass


class Chess(Game):
    def setup(self):
        return "setup chess"

    def take_turns(self):
        return "play chess turns"


class Poker(Game):
    def setup(self):
        return "setup poker"

    def take_turns(self):
        return "play poker turns"


print(Chess().play())
```

### Giải thích ví dụ
- `Game.play()` là template method.
- `setup`, `take_turns`, `tear_down` là các bước trong thuật toán.
- Subclass chỉ thay đổi các bước cần thiết.

## Quy trình hoạt động
1. Client gọi template method trên superclass.
2. Superclass chạy các bước theo thứ tự cố định.
3. Một số bước được subclass override.
4. Luồng tổng thể vẫn không thay đổi.

## Ưu điểm
- Tái sử dụng logic chung hiệu quả.
- Bảo đảm thứ tự các bước quan trọng.
- Giảm trùng lặp giữa các subclass.

## Nhược điểm
- Phụ thuộc vào inheritance.
- Có thể làm subclass bị giới hạn bởi thiết kế superclass.
- Khó thay đổi luồng tổng thể mà không sửa superclass.

## Phân biệt nhanh
- Template Method dùng kế thừa để cố định quy trình.
- Strategy dùng composition để thay đổi thuật toán.
- Hook method cho phép tùy biến nhẹ trong template.

## Lỗi thường gặp
- Override quá nhiều, làm mất ý nghĩa của khung chung.
- Dùng Template Method khi sự khác biệt thực ra nên được tách thành strategy.
- Để các bước trong superclass quá cụ thể, subclass khó tái sử dụng.

## Checklist
- [ ] Có quy trình chung rõ ràng.
- [ ] Các biến thể chỉ khác ở vài bước.
- [ ] Template method kiểm soát thứ tự bước.
- [ ] Subclass chỉ override phần cần thay đổi.

## Tóm tắt ngắn
Template Method cố định khung của thuật toán ở superclass và cho phép subclass tùy biến các bước cụ thể bên trong khung đó.

---

Người soạn: ArchReview AI — Template Method pattern.