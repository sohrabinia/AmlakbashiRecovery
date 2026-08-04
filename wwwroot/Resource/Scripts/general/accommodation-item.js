//function FillRegionChild($this, $childID, $status) {
//    myajax("accomodation/getregionchildrenitems", "region_type=" + $childID + "&parent_id=" + $($this).val() + "&status=" + $status, function (ret) {
//        if (ret.status == 1) {
//            $("select[pid='" + $childID + "']").html(ret.val);
//            var city_region_type = 1;
//            var area_region_type = 2;
//            if ($childID == city_region_type) {
//                $("select[pid='" + area_region_type + "']").html("<option value='-1'>ابتدا شهر را انتخاب کنید</option>");
//            }
//        }
//    });
//}