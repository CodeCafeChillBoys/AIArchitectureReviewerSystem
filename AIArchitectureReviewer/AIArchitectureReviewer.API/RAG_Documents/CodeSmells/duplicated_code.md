# Duplicated Code

## Mô tả
Duplicated Code là việc cùng một đoạn logic, cấu trúc, hoặc cách xử lý bị sao chép ở nhiều nơi trong codebase. Đây là một code smell phổ biến vì nó làm thay đổi trở nên tốn công và dễ lỗi đồng bộ.

Không phải mọi sự lặp đều xấu ở mọi mức độ, nhưng khi cùng một nghiệp vụ hoặc cùng một nhánh xử lý bị copy nhiều lần, đó là tín hiệu cần refactor.

## Ý tưởng cốt lõi
- Một quy tắc nên có một nguồn sự thật duy nhất.
- Sao chép logic làm tăng chi phí thay đổi.
- Nếu hai đoạn code cùng tiến hóa theo một cách, có thể chúng nên được gom lại.

## Dấu hiệu nhận biết
- Cùng một khối logic xuất hiện ở nhiều file hoặc method.
- Copy/paste nhưng đổi tên biến, đổi literal, hoặc đổi thứ tự rất ít.
- Khi sửa một chỗ, phải sửa nhiều chỗ khác.
- Các test giống nhau lặp lại nhiều lần.

## Tác hại
- Bug rất dễ xuất hiện khi một bản sao bị sửa mà bản khác bị quên.
- Làm codebase to hơn mà không tăng giá trị.
- Khó đọc vì cùng một logic lặp đi lặp lại.
- Refactor về sau khó vì số bản sao nhiều.

## Nguyên nhân thường gặp
- Làm nhanh cho kịp deadline.
- Thiếu abstraction phù hợp tại thời điểm viết.
- Không muốn trích xuất method/class vì sợ “over-engineering”.
- Copy code giữa feature tương tự mà không chuẩn hóa thiết kế.

## Ví dụ xấu (Python)
```python
class InvoiceService:
    def total_with_tax(self, amount):
        return amount + amount * 0.1


class ReportService:
    def total_with_tax(self, amount):
        return amount + amount * 0.1
```

Hai method có logic giống hệt nhau. Nếu thuế đổi, cả hai nơi phải sửa.

## Cách xử lý
- Extract Method nếu logic lặp chỉ là một đoạn nhỏ.
- Pull Up Method lên superclass nếu bản sao nằm ở các subclass.
- Introduce Helper/Utility nếu logic thật sự độc lập và dùng nhiều nơi.
- Tạo domain service hoặc policy object nếu logic là nghiệp vụ chung.
- Nếu cấu trúc chỉ tương tự, cân nhắc template method hoặc strategy.

## Hướng refactor thường dùng
- Extract Method.
- Extract Class.
- Pull Up Method / Pull Up Field.
- Template Method.
- Strategy.

## Phân biệt nhanh
- Duplicated Code là logic lặp.
- Data Clumps là dữ liệu lặp theo cụm.
- Shotgun Surgery thường là hậu quả của duplicated code và logic phân tán.

## Checklist
- [ ] Cùng một logic xuất hiện ở nhiều nơi.
- [ ] Sửa một rule phải sửa nhiều bản sao.
- [ ] Có thể trích xuất thành method/class chung.
- [ ] Có thể gom thay đổi vào một điểm duy nhất.

## Tóm tắt ngắn
Duplicated Code làm chi phí thay đổi tăng mạnh và là nguồn gốc của nhiều bug đồng bộ. Cách sửa tốt nhất thường là trích xuất abstraction dùng chung.

---

Người soạn: ArchReview AI — Duplicated Code.