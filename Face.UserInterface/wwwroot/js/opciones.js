document.addEventListener('DOMContentLoaded', function () {
    // Initialize date elements
    const currentDate = new Date();
    updateDateDisplay(currentDate);
    updateFormDateDisplay(currentDate);
    updateLastUpdated();

    // Initialize time input with current time
    const timeInput = document.getElementById('time');
    const currentTime = formatTime(currentDate);
    timeInput.value = currentTime;

    // Initialize date picker
    const dateSelector = document.getElementById('dateSelector');
    const datePicker = flatpickr(dateSelector, {
        defaultDate: currentDate,
        dateFormat: "F j, Y",
        onChange: function (selectedDates) {
            updateDateDisplay(selectedDates[0]);
            updateFormDateDisplay(selectedDates[0]);
        }
    });

    // Form toggle functionality
    const markAttendanceBtn = document.getElementById('markAttendanceBtn');
    const closeFormBtn = document.getElementById('closeFormBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const attendanceFormContainer = document.getElementById('attendanceFormContainer');
    const attendanceTableContainer = document.getElementById('attendanceTableContainer');

    markAttendanceBtn.addEventListener('click', function () {
        attendanceFormContainer.classList.remove('hidden');
        attendanceTableContainer.classList.add('hidden');
    });

    function closeForm() {
        attendanceFormContainer.classList.add('hidden');
        attendanceTableContainer.classList.remove('hidden');
    }

    closeFormBtn.addEventListener('click', closeForm);
    cancelBtn.addEventListener('click', closeForm);

    // Save attendance functionality
    const saveBtn = document.getElementById('saveBtn');
    saveBtn.addEventListener('click', function () {
        // Get form values
        const person = document.getElementById('person').value;
        const time = document.getElementById('time').value;
        const notes = document.getElementById('notes').value;
        const status = document.querySelector('input[name="status"]:checked').value;

        if (!person) {
            alert('Please select a person');
            return;
        }

        // In a real application, you would send this data to a server
        // For this demo, we'll just show a success message and close the form
        alert('Attendance recorded successfully!');
        updateLastUpdated();
        closeForm();
    });
});

function updateDateDisplay(date) {
    const selectedDate = document.getElementById('selectedDate');
    selectedDate.textContent = formatDate(date);
}

function updateFormDateDisplay(date) {
    const formDate = document.getElementById('formDate');
    formDate.textContent = `Recording attendance for ${formatDate(date)}`;
}

function updateLastUpdated() {
    const lastUpdated = document.getElementById('lastUpdated');
    const now = new Date();
    lastUpdated.textContent = `Last updated: ${formatTime12Hour(now)}`;
}

function formatDate(date) {
    const options = { month: 'long', day: 'numeric', year: 'numeric' };
    return date.toLocaleDateString('en-US', options);
}

function formatTime(date) {
    return `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
}

function formatTime12Hour(date) {
    const hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const hour12 = hours % 12 || 12;

    return `${hour12}:${String(minutes).padStart(2, '0')} ${ampm}`;
}