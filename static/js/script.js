const menuBtn = $('.header-nav-more__btn'),
      menu    = $('.header-nav-dropdown'),
      hamMenuBtn = $('.header__menu'),
      hamMenu = $('.header-hum-menu');
let is_open = false;

menuBtn.on('click', function() {
  if ( $(this).hasClass('is-active') ) {
      $(this).removeClass('is-active');
      menu.slideUp();
  } else {
      $(this).addClass('is-active');
      menu.slideDown();
  }
});

$(document).scroll(function (e) {
  if ( !menuBtn.is(e.target) && !menu.is(e.target) && menu.has(e.target).length === 0) {
      menu.slideUp();
      menuBtn.removeClass('is-active');
  };
});

$('.header__menu').click(function() {
    $('.header-ham-menu').animate({right: '0px'}, 200);
    $('body').animate({ right: '250px' }, 200);
    is_open = true;
});

$('.header__close').click(function() {
    $('.header-ham-menu').animate({ right: '-250px'}, 200);
    $('body').animate({right: '0px'}, 200);
    is_open = false;
});

$('.footer__partner').css({
    "background-image": 'url('+($(this).attr('data-img-bg')+')'),
});



$('.footer__partner__img').mouseover(function(){
    $(this).stop().animate({opacity:'1.0'},200);
});
$('.footer__partner__img').mouseout(function(){
    $(this).stop().animate({opacity:'0'},200);
});

$('#select_league').on('click', function() {
    $('#select_league').toggleClass('teams__selected');
  });