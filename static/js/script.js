const menuBtn = $('.header-nav-more__btn'),
      menu    = $('.header-nav-dropdown'),
      hamMenuBtn = $('.header__menu'),
      hamMenu = $('.header-hum-menu'),
      matchesTable = $('.player-matches__header');
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

document.getElementById("defaultOpen").click();

function openBlock(event, blockName, line) {
    // Declare all variables
    var i, tabcontent, tablinks, lines;
    lines = document.getElementsByClassName("line");
    for (i = 0; i < lines.length; i++) {
        lines[i].style.display = "none";
    }
    
    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tab");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(blockName).style.display = "block";
    document.getElementById(line).style.display = "block";
    event.currentTarget.className += " active";
}