var bCircle = [[10,10,10,100,50], [10,10,10,10,20], [10,50,21,20,30],[0,0,0,0,38], [0,0,0,0,42], [10,50,21,20,34]];
//var bCircle = [[0,0,0,0,2], [0,0,0,0,2], [0,0,0,0,2],[0,0,0,0,2], [0,0,0,0,2], [0,0,0,0,2]];

jQuery(document).ready(function() {

	i = 0;

	jQuery('.header .circleimg').each(function() {
		bCircle[i][0] = center(jQuery(this).offset(), jQuery(this).width());
		bCircle[i][1] = center(jQuery(this).offset(), jQuery(this).height());

		bCircle[i][2] = parseInt(jQuery(this).css('left'));
		bCircle[i][3] = parseInt(jQuery(this).css('top'));
		i++;
		if (i>4) i = 0;
	});

	jQuery('.header .section-bg').mousemove(function(e) {
		i = 0;
		jQuery(this).find('.circleimg').each(function() {
			dX = (e.pageX - bCircle[i][0])/(bCircle[i][4]);
			dY = (e.pageY - bCircle[i][1])/(bCircle[i][4]);

			jQuery(this).css('left', bCircle[i][2]+dX);
			jQuery(this).css('top', bCircle[i][3]+dY);

			i++;
			if (i>4) i = 0;
		});
	});
});

function center (offset, dim) {
	res = offset.left + dim / 2;
	return res;
}
