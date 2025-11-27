// JavaScript Document

//$('.spctext').click(function(){
//	$('.copytext').addClass('disnone');	
//	$('.footer').addClass('minhight');  
//	$('#subnavmain').addClass('default');	
//	 
//	  $('.default').perfectScrollbar({ suppressScrollX: true }); 
//	  });
//$('.text-active').click(function(){
//		   $('.footer').removeClass('minhight');
//	$('.copytext').removeClass('disnone');	
//	$('#subnavmain').removeClass('default');  
//	  });

$(document).ready(function(){
  $('input[type=file]').bootstrapFileInput();
});
	  
	
	$('.btncollapse').click(function(){
			$('.copytext').toggleClass('disnone');	
			$('.footer').toggleClass('minhight');  
			$('#subnavmain').toggleClass('default');	
			 
			  $('.default').perfectScrollbar({ suppressScrollX: true }); 
 });
			  
	 