let animations = new WeakMap();

export function initialize(root, path, measure, head, tail, options) {
    if (!path || !measure) {
        return {
            length: 0,
            reps: 1
        };
    }

    let length = 0;
    let unitWidth = 0;

    try {
        length = path.getTotalLength();
        unitWidth = measure.getComputedTextLength();
    } catch {
        return {
            length: 0,
            reps: 1
        };
    }

    if (!length) {
        return {
            length: 0,
            reps: 1
        };
    }

    const reps = unitWidth > 0
    ? Math.max(3, Math.ceil(length / unitWidth) + 5)
    : 3;

    return {
        length,
        reps
    };
}

export function start(
    root,
    head,
    tail,
    length,
    speed,
    direction,
    pauseOnHover
) {
    if (!root || !head || !tail || !length || speed <= 0) {
        return;
    }

    dispose(root);

    let offset = 0;
    let animationFrame = null;
    let lastTime = performance.now();
    let paused = false;

    const prefersReducedMotion =
        window.matchMedia(
            "(prefers-reduced-motion: reduce)"
        ).matches;

    const apply = value => {
        const partner =
            value >= 0
                ? value - length
                : value + length;

        head.setAttribute(
            "startOffset",
            String(value)
        );

        tail.setAttribute(
            "startOffset",
            String(partner)
        );
    };

    apply(0);

    if (prefersReducedMotion) {
        return;
    }

    const directionMultiplier =
        direction === "reverse"
            ? -1
            : 1;

    const animate = now => {
        if (paused) {
            lastTime = now;
            animationFrame =
                requestAnimationFrame(animate);
            return;
        }

        const delta =
            (now - lastTime) / 1000;

        lastTime = now;

        offset +=
            speed *
            delta *
            directionMultiplier;

        if (offset >= length) {
            offset -= length;
        }

        if (offset <= -length) {
            offset += length;
        }

        apply(offset);

        animationFrame =
            requestAnimationFrame(animate);
    };

    animationFrame =
        requestAnimationFrame(animate);

    const pause = () => {
        paused = true;
    };

    const resume = () => {
        paused = false;
        lastTime = performance.now();
    };

    if (pauseOnHover) {
        root.addEventListener(
            "pointerenter",
            pause
        );

        root.addEventListener(
            "pointerleave",
            resume
        );
    }

    animations.set(root, {
        frame: animationFrame,
        pause,
        resume
    });
}

export function dispose(root) {
    const animation = animations.get(root);

    if (!animation) {
        return;
    }

    cancelAnimationFrame(animation.frame);

    root.removeEventListener(
        "pointerenter",
        animation.pause
    );

    root.removeEventListener(
        "pointerleave",
        animation.resume
    );

    animations.delete(root);
}