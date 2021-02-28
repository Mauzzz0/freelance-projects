const menuBtn = $('.header-nav-more__btn'),
      menu    = $('.header-nav-dropdown'),
      hamMenuBtn = $('.header__menu'),
      hamMenu = $('.header-hum-menu'),
      matchesTable = $('.player-matches__header');
let is_open = false;

menuBtn.on('click', function() {
  if ( $(this).hasClass('is-active') ) {
      $(this).removeClass('is-active');
      menu.slideUp(200);
  } else {
      $(this).addClass('is-active');
      menu.slideDown(200);
  }
});

matchesTable.on('click', function() {
    $(this).next($('.player-matches-table-block')).toggleClass('hidden');
    if ( $(this).hasClass('is-active') ) {
        $(this).removeClass('is-active');
        $(this).children('i').css('transform', 'rotate(0deg)');
    } else {
        $(this).addClass('is-active');
        $(this).children('i').css('transform', 'rotate(180deg)');
    }
    
  });

$(document).scroll(function (e) {
  if ( !menuBtn.is(e.target) && !menu.is(e.target) && menu.has(e.target).length === 0) {
      menu.slideUp(200);
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

$('.footer__partner__img').mouseover(function(){
    $(this).stop().animate({opacity:'1.0'},200);
});
$('.footer__partner__img').mouseout(function(){
    $(this).stop().animate({opacity:'0'},200);
});

$('.message').on('click', ()=>{
    $('.message').toggleClass("hidden");
})


