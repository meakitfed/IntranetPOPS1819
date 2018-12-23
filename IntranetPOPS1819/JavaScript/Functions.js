function toggleVisibility(controlId)
{
    var control = document.getElementById(controlId);

	if (control.style.display == "none")
		control.style.display = "";
    else
		control.style.display = "none";
}