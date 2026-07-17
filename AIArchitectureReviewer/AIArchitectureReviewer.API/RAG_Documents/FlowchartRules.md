# Quy tắc Đánh giá và Hướng dẫn Chi tiết Sơ đồ Hoạt động / Sơ đồ Luồng (Activity / Flowchart Diagram)

Tài liệu này cung cấp định nghĩa chuẩn hóa, các ký pháp thành phần, làn bơi phân vai (swimlanes) và quy tắc đánh giá chi tiết cho Sơ đồ Hoạt động (Activity Diagram) dựa trên tiêu chuẩn UML.

---

## I. THÀNH PHẦN VÀ KÝ PHÁP CHÍNH (DIAGRAM SYMBOLS)

Sơ đồ Hoạt động biểu diễn luồng điều khiển nghiệp vụ hoặc thuật toán, mô tả trình tự các bước thực thi, các điều kiện rẽ nhánh và các tác vụ chạy song song. Sơ đồ gồm các ký pháp chính sau:

1. **Nút khởi đầu (Start / Initial Node)**:
   - Điểm bắt đầu của luồng hoạt động. Sơ đồ bắt buộc phải có duy nhất một nút khởi đầu.
   - Ký pháp: Hình tròn đen đặc đơn giản.

2. **Hành động / Trạng thái (Activity / Action State)**:
   - Đại diện cho một tác vụ, bước thực thi cụ thể trong quy trình. Nhãn phải ngắn gọn và bắt đầu bằng động từ hướng hành động (ví dụ: "Duyệt đơn hàng", "Gửi Email").
   - Ký pháp: Hình chữ nhật bo tròn góc.

3. **Luồng điều khiển (Control Flow)**:
   - Mũi tên chỉ hướng chuyển tiếp thực thi từ hành động này sang hành động khác.
   - Ký pháp: Đường nét liền có đầu mũi tên nhọn.

4. **Nút rẽ nhánh quyết định (Decision Node)**:
   - Điểm phân nhánh luồng dựa trên điều kiện logic. Có 1 đầu vào và nhiều đầu ra. Mỗi đầu ra phải ghi rõ nhãn điều kiện bảo vệ (guard condition - ví dụ: "[Hợp lệ]" / "[Không hợp lệ]").
   - Ký pháp: Hình thoi đơn giản.

5. **Nút gộp luồng (Merge Node)**:
   - Điểm gộp nhiều nhánh rẽ từ Decision Node quay trở lại một luồng chính. Có nhiều đầu vào và 1 đầu ra.
   - Ký pháp: Hình thoi (không ghi nhãn điều kiện ở các nhánh vào).

6. **Thanh chia nhánh song song (Fork Node)**:
   - Chia một luồng thực thi thành hai hoặc nhiều luồng chạy song song đồng thời.
   - Ký pháp: Một thanh ngang hoặc dọc màu đen đặc dày.

7. **Thanh hội tụ song song (Join Node)**:
   - Đồng bộ hóa các luồng chạy song song từ Fork Node quay trở lại làm một luồng duy nhất. Luồng ra chỉ kích hoạt khi tất cả các luồng vào song song đã hoàn thành.
   - Ký pháp: Một thanh ngang hoặc dọc màu đen đặc dày.

8. **Nút kết thúc hoạt động (Activity Final Node)**:
   - Đánh dấu điểm kết thúc của toàn bộ các luồng trong sơ đồ.
   - Ký pháp: Hình tròn đen đặc có vòng tròn bao quanh ở ngoài (dạng hồng tâm).

9. **Nút kết thúc luồng đơn (Flow Final Node)**:
   - Đánh dấu kết thúc của một nhánh chạy đơn lẻ mà không làm dừng toàn bộ sơ đồ hoạt động.
   - Ký pháp: Hình tròn có chữ X ở giữa.

---

## II. LÀN BƠI PHÂN CHIA TRÁCH NHIỆM (SWIMLANES / PARTITIONS)

Làn bơi (Swimlanes) được sử dụng để phân nhóm các hành động dựa trên vai trò chịu trách nhiệm của từng Actor (người dùng, hệ thống, phòng ban):
- Giúp người đọc dễ dàng thấy được hành động nào do ai thực hiện và các điểm bàn giao (handoffs) giữa các bên.
- **Quy tắc**: Không nên thiết kế quá 5 làn bơi trong một sơ đồ để tránh rối mắt. Sắp xếp các làn bơi theo trình tự luồng logic tương tác từ trái qua phải hoặc từ trên xuống dưới.

---

## III. BỘ TIÊU CHUẨN KIỂM TRA LỖI KIẾN TRÚC CHO AI (CHECKLIST)

AI bắt buộc phải rà soát cấu trúc JSON và phân tích sơ đồ Hoạt động dựa trên các tiêu chuẩn kiểm định sau:

1. **Ngõ cụt logic (Dead-end Node)**:
   * **Dấu hiệu**: Một nút hành động (Action) không có bất kỳ mũi tên luồng điều khiển đầu ra nào để đi tiếp đến nút kết thúc.
   * **Cách kiểm tra**: Quét danh sách các nút hành động trong JSON, kiểm tra xem có nút nào không nằm trong tập hợp nguồn (Source) của bất kỳ kết nối nào hay không.
   * **Giải pháp**: Yêu cầu kết nối nút ngõ cụt đó đến nút kết thúc (Activity Final Node) hoặc đến bước tiếp theo thích hợp.

2. **Mập mờ điều kiện rẽ nhánh (Ambiguous Guard Conditions)**:
   * **Dấu hiệu**: Nút quyết định hình thoi (Decision Node) có các mũi tên đầu ra nhưng không ghi nhãn điều kiện bảo vệ (ví dụ: Có/Không, Thành công/Thất bại), hoặc thiếu một nhánh logic quan trọng (ví dụ: chỉ vẽ nhánh thành công mà quên nhánh xử lý khi thất bại).
   * **Cách kiểm tra**: Rà soát các nút Decision, kiểm tra xem các luồng ra có được gắn nhãn text đầy đủ hay không.
   * **Giải pháp**: Yêu cầu bổ sung nhãn điều kiện rõ ràng (bọc trong ngoặc vuông `[Điều kiện]`) cho mọi nhánh ra và bổ sung nhánh ngoại lệ nếu thiếu.

3. **Mất cân bằng song song (Fork/Join Mismatch)**:
   * **Dấu hiệu**: Mở luồng chạy song song bằng thanh Fork Node nhưng các luồng con này không bao giờ được đồng bộ lại bằng thanh Join Node, hoặc số lượng nhánh vào/ra ở thanh Fork/Join bị lệch pha tạo ra lỗi rò rỉ luồng chạy.
   * **Cách kiểm tra**: Tìm các nút Fork và đối chiếu xem các nhánh song song tỏa ra từ đó có quy tụ về một nút Join tương ứng trước khi kết thúc hay không.
   * **Giải pháp**: Yêu cầu chèn thanh Join Node để đồng bộ hóa và hợp nhất các luồng chạy song song trước khi chuyển sang bước tiếp theo.

4. **Đặt nhãn hành động mơ hồ (Vague Action Labels)**:
   * **Dấu hiệu**: Nhãn của các nút hành động ghi chung chung không mang tính hành động rõ ràng (ví dụ: "Xử lý dữ liệu", "Hệ thống", "Bước 2") thay vì một nhãn ngắn gọn hướng hành động.
   * **Cách kiểm tra**: Kiểm tra văn bản nhãn của các nút Action.
   * **Giải pháp**: Yêu cầu sửa đổi nhãn bắt đầu bằng một động từ mạnh mẽ mô tả hành động (ví dụ: "Kiểm tra tính hợp lệ", "Ghi nhận thanh toán").

5. **Trộn lẫn quá nhiều kịch bản (Scenario Overload)**:
   * **Dấu hiệu**: Sơ đồ cố gắng nhồi nhét quá nhiều luồng biến thể nghiệp vụ phức tạp không liên quan vào chung một bản vẽ khiến luồng đi chằng chịt, giao nhau chồng chéo.
   * **Giải pháp**: Khuyên người dùng nên tách luồng thành một kịch bản chính rõ ràng và vẽ các luồng con ở sơ đồ hoạt động phụ trợ riêng biệt.