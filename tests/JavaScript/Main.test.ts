import { equal } from 'assert';
import { expect, it, describe } from 'vitest';

describe('Mocha native', function () {
    describe('subtestlist', function () {
        it('should return -1 when the value is not present', function () {
            equal([1, 2, 3].indexOf(4), -1);
        });
    });
});