import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '5s', target: 10 }, // ramp up to 10
        { duration: '15s', target: 50 },  // ramp up to 50
        { duration: '20s', target: 150 }, // ramp up to 150
        { duration: '15s', target: 50 },  // ramp down to 50 users
        { duration: '5s', target: 0 },  // ramp down to 0 users
    ],
};

export default function () {
    let res = http.get('http://localhost:8080/simple/date-time');
    check(res, {
        'is status 200': (r) => r.status === 200,
        'is content-type application/json': (r) => r.headers['Content-Type'] === 'application/json',
    });
    sleep(1);
}