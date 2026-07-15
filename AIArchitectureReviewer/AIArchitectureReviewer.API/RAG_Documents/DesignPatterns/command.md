# Command Pattern

## Mô tả
Command là behavioral pattern đóng gói một yêu cầu hoặc hành động thành một object riêng biệt. Nhờ đó, client, invoker, và receiver được tách rời, đồng thời hệ thống có thể hỗ trợ queue, log, retry, undo, redo, hoặc macro commands.

## Ý tưởng cốt lõi
- Biến một hành động thành object.
- Tách người ra lệnh khỏi người thực thi.
- Cho phép lưu, xếp hàng, truyền đi, hoặc hoàn tác lệnh.

## Cấu trúc tham gia
- Command: interface cho các lệnh.
- Concrete Command: đóng gói hành vi cụ thể.
- Receiver: đối tượng thực sự thực hiện công việc.
- Invoker: gọi command mà không biết chi tiết bên trong.
- Client: tạo command và gắn receiver vào command.

## Khi nào dùng
- Khi cần hỗ trợ undo/redo.
- Khi muốn xếp hàng hoặc lên lịch các thao tác.
- Khi cần ghi log các hành động người dùng.
- Khi muốn tách UI/button/menu khỏi logic nghiệp vụ.

## Khi không nên dùng
- Khi hành động quá đơn giản và không cần lưu dưới dạng object.
- Khi không có nhu cầu queue, undo, macro, hoặc phân tách invoker/receiver.

## Ví dụ (Python)
Ví dụ dưới đây điều khiển đèn bằng command object.

```python
from abc import ABC, abstractmethod


class Light:
    def on(self):
        return "light on"

    def off(self):
        return "light off"


class Command(ABC):
    @abstractmethod
    def execute(self):
        pass

    def undo(self):
        pass


class LightOnCommand(Command):
    def __init__(self, light):
        self.light = light

    def execute(self):
        return self.light.on()

    def undo(self):
        return self.light.off()


class LightOffCommand(Command):
    def __init__(self, light):
        self.light = light

    def execute(self):
        return self.light.off()

    def undo(self):
        return self.light.on()


class Invoker:
    def __init__(self):
        self.history = []

    def run(self, command):
        result = command.execute()
        self.history.append(command)
        return result

    def undo_last(self):
        if not self.history:
            return None
        command = self.history.pop()
        return command.undo()


light = Light()
remote = Invoker()
print(remote.run(LightOnCommand(light)))
print(remote.undo_last())
```

### Giải thích ví dụ
- `Light` là receiver.
- `LightOnCommand` và `LightOffCommand` là concrete commands.
- `Invoker` chỉ gọi command mà không biết receiver là gì.

## Quy trình hoạt động
1. Client tạo command và gắn receiver vào command.
2. Invoker nhận command.
3. Invoker gọi `execute()`.
4. Command chuyển lời gọi sang receiver.
5. Nếu cần, invoker có thể lưu command để undo/redo.

## Ưu điểm
- Tách biệt rõ ràng giữa gọi và thực thi.
- Dễ thêm lệnh mới mà ít ảnh hưởng tới invoker.
- Hỗ trợ undo/redo, queue, logging.
- Phù hợp cho UI action và workflow phức tạp.

## Nhược điểm
- Tăng số lượng class.
- Có thể làm code verbose nếu lệnh đơn giản.
- Cần quản lý state cẩn thận để undo hoạt động đúng.

## Phân biệt nhanh
- Command đóng gói hành động.
- Strategy đóng gói thuật toán để hoán đổi.
- Observer phát tán sự kiện đến nhiều listener.

## Lỗi thường gặp
- Command biết quá nhiều về UI hoặc invoker.
- Không lưu đủ state để undo.
- Gộp mọi logic vào invoker thay vì để command tự chứa hành vi.

## Checklist
- [ ] Hành động được đóng gói thành object.
- [ ] Invoker không phụ thuộc vào receiver cụ thể.
- [ ] Có thể lưu lịch sử hoặc xếp hàng lệnh.
- [ ] Undo/redo nếu cần đã được thiết kế từ đầu.

## Tóm tắt ngắn
Command biến thao tác thành object độc lập, giúp hệ thống linh hoạt hơn khi cần queue, log, undo, hoặc tách UI khỏi business logic.

---

Người soạn: ArchReview AI — Command pattern.