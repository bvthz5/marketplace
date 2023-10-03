export function convertDate(dates) {
  let date = new Date(dates);
  if (isNaN(date.getTime())) {
    return "";
  }
  let month = ['JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'];
  return date.getDate() + ' ' + month[date.getMonth()] + ' ' + date.getFullYear().toString();
}
export  function convertDatesmall(dates) {
  let date = new Date(dates);
  if (isNaN(date.getTime())) {
    return "";
  }
  let month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  return date.getDate() + ' ' + month[date.getMonth()] + ' ' + date.getFullYear().toString();
}


export  const getRelativeDate=(date)=> {
  const today = new Date();
  const inputDate = new Date(date);
  const oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
  const diffDays = Math.round(Math.abs((today - inputDate) / oneDay));

  if (diffDays === 0) {
    return 'TODAY';
  } else if (diffDays === 1) {
    return 'YESTERDAY';
  } else if (diffDays < 7) {
    return `${diffDays} DAYS AGO`;
  } else {
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const month = monthNames[inputDate.getMonth()];
    const day = inputDate.getDate();
    return `${month.toUpperCase()} ${day}`;
  }
}
