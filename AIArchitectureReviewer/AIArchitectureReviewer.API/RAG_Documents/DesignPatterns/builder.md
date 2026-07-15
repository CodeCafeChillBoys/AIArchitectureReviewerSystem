# Builder Pattern

## Mô tả
Builder là creational pattern tách quá trình tạo một đối tượng phức tạp ra khỏi biểu diễn cuối cùng của nó. Thay vì khởi tạo object bằng một constructor dài với nhiều tham số, Builder cho phép dựng object theo từng bước rõ ràng.

Pattern này đặc biệt hữu ích khi một object có nhiều phần tùy chọn, nhiều cấu hình có thể thay đổi, hoặc việc dựng object cần theo một trình tự nhất định.

## Ý tưởng cốt lõi
- Xây dựng object theo từng bước.
- Tách logic dựng object khỏi object cuối cùng.
- Cho phép tạo nhiều biến thể của cùng một cấu trúc.
- Làm cho code khởi tạo dễ đọc và dễ bảo trì hơn.

## Cấu trúc tham gia
- Builder: định nghĩa các bước xây dựng.
- Concrete Builder: triển khai từng bước.
- Product: object cuối cùng được tạo ra.
- Director: điều phối thứ tự xây dựng nếu cần.
- Client: gọi builder để tạo sản phẩm.

## Khi nào dùng
- Khi object có nhiều tham số tùy chọn hoặc cấu hình phức tạp.
- Khi muốn tránh constructor dài và khó đọc.
- Khi cần tạo object theo nhiều biến thể khác nhau.
- Khi cần đảm bảo các bước khởi tạo diễn ra theo thứ tự hợp lệ.

## Khi không nên dùng
- Khi object rất đơn giản và chỉ có vài tham số.
- Khi constructor/factory function đã đủ rõ ràng.
- Khi thêm Builder chỉ khiến code rườm rà hơn.

## Ví dụ (Python)
Ví dụ dưới đây xây dựng một ngôi nhà theo từng bước.

```python
from dataclasses import dataclass, field


@dataclass
class House:
    walls: int = 0
    roof: bool = False
    doors: int = 0
    windows: int = 0
    extras: list[str] = field(default_factory=list)


class HouseBuilder:
    def __init__(self):
        self.reset()

    def reset(self):
        self._house = House()
        return self

    def add_walls(self, count=4):
        self._house.walls = count
        return self

    def add_roof(self):
        self._house.roof = True
        return self

    def add_doors(self, count=1):
        self._house.doors = count
        return self

    def add_windows(self, count=2):
        self._house.windows = count
        return self

    def add_extra(self, item):
        self._house.extras.append(item)
        return self

    def build(self):
        product = self._house
        self.reset()
        return product


house = (
    HouseBuilder()
    .add_walls(4)
    .add_roof()
    .add_doors(2)
    .add_windows(6)
    .add_extra("solar panel")
    .build()
)
print(house)
```

### Giải thích ví dụ
- `House` là product cuối cùng.
- `HouseBuilder` giữ từng bước dựng object.
- Các method builder trả về chính nó để hỗ trợ fluent API.
- `build()` trả về product đã hoàn thiện và reset builder để dùng lại.

## Quy trình hoạt động
1. Client tạo builder.
2. Client gọi từng bước dựng object.
3. Builder lưu trạng thái tạm trong quá trình dựng.
4. `build()` trả về object hoàn chỉnh.

## Ưu điểm
- Code khởi tạo rõ ràng hơn.
- Giảm constructor phức tạp.
- Hỗ trợ tạo object từng phần và nhiều biến thể.
- Dễ thêm bước mới mà ít ảnh hưởng đến client.

## Nhược điểm
- Tăng số lượng class/method.
- Có thể quá mức cho object đơn giản.
- Nếu builder giữ state không cẩn thận, dễ gây lỗi tái sử dụng.

## Phân biệt nhanh
- Builder tập trung vào cách dựng object.
- Factory Method tập trung vào chọn lớp để khởi tạo.
- Abstract Factory tập trung vào tạo ra cả một họ object liên quan.

## Lỗi thường gặp
- Không reset state sau khi build.
- Dùng Builder cho object đơn giản.
- Để builder biết quá nhiều về nghiệp vụ thay vì chỉ dựng cấu trúc.

## Checklist
- [ ] Object có nhiều phần tùy chọn hoặc phức tạp.
- [ ] Quá trình dựng object được tách riêng khỏi product.
- [ ] Fluent API hoặc từng bước xây dựng rõ ràng.
- [ ] `build()` trả về object hoàn chỉnh.

## Tóm tắt ngắn
Builder phù hợp khi bạn cần dựng một object phức tạp theo từng bước và muốn giữ cho code khởi tạo rõ ràng, linh hoạt.

---

Người soạn: ArchReview AI — Builder pattern.