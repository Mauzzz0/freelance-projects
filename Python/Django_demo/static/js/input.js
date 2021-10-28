$(document).ready( function() {
  $("#createTeam-field__logo").change(function(){
       var filename = $(this).val().replace(/.*\\/, "");
       $("#createTeam-field__logo-text").html(filename);
  });
});

$(document).ready( function() {
  $("#createTeam-field__players").change(function(){
       var filename = $(this).val().replace(/.*\\/, "");
       $("#createTeam-field__players-text").html(filename);
  });
});