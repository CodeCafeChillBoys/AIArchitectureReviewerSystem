# Facade Pattern

## Mô tả
Facade là structural pattern cung cấp một giao diện cấp cao, đơn giản hơn cho một subsystem phức tạp. Thay vì để client làm việc trực tiếp với nhiều class và nhiều bước gọi khác nhau, facade gom các thao tác đó lại thành một API dễ dùng hơn.

Facade không thay thế subsystem; nó chỉ là lớp “cửa trước” giúp client thao tác gọn hơn.

## Ý tưởng cốt lõi
- Che giấu độ phức tạp của subsystem.
- Cung cấp API đơn giản, ổn định cho client.
- Giảm coupling giữa client và các class bên trong hệ thống.

## Cấu trúc tham gia
- Facade: interface đơn giản cho client.
- Subsystem classes: các class thực hiện công việc thật.
- Client: chỉ gọi facade.

## Khi nào dùng
- Khi subsystem có nhiều bước gọi rối rắm.
- Khi muốn giảm số lượng phụ thuộc trực tiếp của client.
- Khi cần một API mức cao cho use case phổ biến.

## Khi không nên dùng
- Khi subsystem vốn đã nhỏ và đơn giản.
- Khi facade che giấu quá nhiều khiến client khó kiểm soát chi tiết cần thiết.

## Ví dụ (Python)
Ví dụ dưới đây gom nhiều bước khởi động hệ thống thành một lời gọi duy nhất.

```python
class CPU:
    def freeze(self):
        return "CPU freeze"

    def jump(self, position):
        return f"CPU jump to {position}"

    def execute(self):
        return "CPU execute"


class Memory:
    def load(self, position, data):
        return f"Memory load {data} at {position}"


class Disk:
    def read(self, lba, size):
        return f"Disk read {size} bytes from {lba}"


class ComputerFacade:
    def __init__(self):
        self.cpu = CPU()
        self.memory = Memory()
        self.disk = Disk()

    def start(self):
        steps = [
            self.cpu.freeze(),
            self.memory.load(0, self.disk.read(0, 1024)),
            self.cpu.jump(0),
            self.cpu.execute(),
        ]
        return steps


computer = ComputerFacade()
print(computer.start())
```

### Giải thích ví dụ
- `CPU`, `Memory`, `Disk` là các subsystem.
- `ComputerFacade` gom thứ tự thao tác phức tạp vào `start()`.
- Client chỉ cần gọi một method thay vì tự điều phối toàn bộ subsystem.

## Quy trình hoạt động
1. Client gọi method trên facade.
2. Facade điều phối các subsystem theo đúng thứ tự.
3. Kết quả được trả về theo dạng đơn giản hơn cho client.

## Ưu điểm
- Giảm độ phức tạp khi sử dụng subsystem.
- Làm code client ngắn gọn và dễ đọc hơn.
- Giảm phụ thuộc vào chi tiết bên trong subsystem.

## Nhược điểm
- Có thể trở thành lớp “god object” nếu gom quá nhiều trách nhiệm.
- Nếu thiết kế không tốt, facade có thể làm mất một số khả năng tinh chỉnh của client.

## Phân biệt nhanh
- Facade đơn giản hóa subsystem.
- Adapter chuyển đổi interface không tương thích.
- Decorator thêm hành vi cho object.

## Lỗi thường gặp
- Để facade chứa quá nhiều business logic.
- Biến facade thành nơi làm tất cả mọi thứ thay vì chỉ điều phối.
- Tạo facade nhưng vẫn để client truy cập subsystem trực tiếp, làm mất mục tiêu giảm coupling.

## Checklist
- [ ] Client có thể dùng subsystem qua một interface đơn giản.
- [ ] Facade chủ yếu điều phối, không ôm toàn bộ nghiệp vụ.
- [ ] Subsystem vẫn có thể dùng trực tiếp nếu thật sự cần.

## Tóm tắt ngắn
Facade giúp ẩn sự phức tạp của subsystem và cung cấp một điểm vào dễ dùng hơn cho client.

---

Người soạn: ArchReview AI — Facade pattern.