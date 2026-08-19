window.submitGoogleRsvp = async function (
    formUrl,
    nameEntryId,
    name,
    attendanceEntryId,
    attendance
) {
    const formData = new FormData();

    formData.append(nameEntryId, name);
    formData.append(attendanceEntryId, attendance);

    // Tell Google Forms that we went through Section 1 and Section 2
    formData.append("pageHistory", "0,1");

    await fetch(formUrl, {
        method: "POST",
        mode: "no-cors",
        body: formData
    });
};