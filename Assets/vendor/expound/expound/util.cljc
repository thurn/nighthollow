(ns ^:no-doc expound.util)

(def assert-message "Internal Expound assertion failed. Please report this bug at https://github.com/bhb/expound/issues")

(defn nan? [x]
  #?(:clj (and (number? x) (Double/isNaN x))
     :cljr (and (number? x) (Double/IsNaN x))
     :cljs (and (number? x) (js/isNaN x))))
