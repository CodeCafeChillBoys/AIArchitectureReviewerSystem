# Extract Class — Tách lớp (Refactoring)

## Mô tả
Extract Class là kỹ thuật tách một phần trách nhiệm từ một lớp lớn (God Class) thành một lớp mới có trách nhiệm rõ ràng hơn. Mục tiêu là giảm coupling và tăng cohesion.

## Khi nào dùng
- Lớp có nhiều trường/method không liên quan.
- Có nhóm phương thức thao tác trên cùng một tập dữ liệu nội bộ.
- Thay đổi về một phần chức năng thường xuyên khiến phải sửa lớp lớn.

## Các bước thực hiện
1. Xác định nhóm thuộc tính và phương thức có thể tách ra.
2. Tạo lớp mới (NewClass) và di chuyển thuộc tính/phương thức vào.
3. Thiết lập giao diện giữa hai lớp (aggregation/composition) — thường lớp gốc sẽ giữ tham chiếu đến lớp mới.
4. Cập nhật nơi sử dụng, viết test cho lớp mới.
5. Refactor tiếp để tối ưu visibility (private/public) và dependency.

## Ví dụ (Python)
```python
# Before: Order có logic tính thuế và lưu trữ
class Order:
    def __init__(self):
        self.items = []
    def calculate_tax(self):
        # nhiều dòng code
        pass
    def save(self):
        # lưu DB
        pass

# After: tách TaxCalculator và Repository
class TaxCalculator:
    def calculate(self, order):
        pass

class OrderRepository:
    def save(self, order):
        pass

class Order:
    def __init__(self, tax_calculator, repo):
        self.items = []
        self.tax_calculator = tax_calculator
        self.repo = repo
    def calculate_tax(self):
        return self.tax_calculator.calculate(self)
    def save(self):
        self.repo.save(self)
```

## Checklist
- [ ] Có test trước và sau refactor để đảm bảo không làm thay đổi behavior.
- [ ] Đảm bảo không kéo theo vòng phụ thuộc (circular dependency).
- [ ] Tên lớp/thuộc tính rõ ràng, phản ánh trách nhiệm.

## Lưu ý
- Refactor từ từ: tách từng phần nhỏ và chạy test liên tục.
- Đôi khi extract thành module thay vì class là phù hợp hơn.

---

Người soạn: ArchReview AI — Hướng dẫn Extract Class.