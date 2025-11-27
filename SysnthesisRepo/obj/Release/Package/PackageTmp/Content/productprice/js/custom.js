/* ========== Navigation mobile view js Strat ========== */ 
$(document).ready(function () {

      //Burger Menu JS
      $('#dismiss, .overlay').on('click', function () {
          $('#sidebar').removeClass('active');
          $('.overlay').fadeOut();
      });
      $('#sidebarCollapse').on('click', function () {
          $('#sidebar').toggleClass('active');
          $('.overlay').fadeIn();
          $('.collapse.in').toggleClass('in');
          $('a[aria-expanded=true]').attr('aria-expanded', 'false');
      });



      //Multiple Dropdown Click JS
      $("ul.dropdown-menu [data-toggle='dropdown']").on("click", function(event) {
          event.preventDefault();
          event.stopPropagation();

          $(this).siblings().toggleClass("show");
      
          if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
          }
          $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function(e) {
            $('.submenu .show').removeClass("show");
          });
      });  

});
/* ========== Navigation mobile view js Strat ========== */
